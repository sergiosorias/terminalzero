using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration.Controls;
using ZeroConfiguration.Entities;
using ZeroConfiguration.Properties;


namespace ZeroConfiguration
{
    public partial class ZeroConfigurationModule : ZeroModule, ITerminalManager
    {
        private Synchronizer Sync = null;
        public ZeroAction SyncAction = null;

        public ZeroConfigurationModule(ITerminal iCurrentTerminal)
            : base(iCurrentTerminal, 2, "Administra configuraciones sobre las terminales")
        {
            BuildPosibleActions();
            BuildRulesActions();
        }

        private void BuildPosibleActions()
        {
            //actions.Add(new ZeroAction(ActionType.MenuItem, "Reload", (rule) => { OnConfigurationRequired(); }));

            SyncAction = new ZeroAction(ActionType.BackgroudAction, "Configuración@Sincronizar", StartSync);
            SyncAction.Parameters.Add(new ZeroActionParameterBase(typeof(ISyncService), true));
            SyncAction.Parameters.Add(new ZeroActionParameterBase("ExistingModules", true));
            Terminal.Session.AddAction(SyncAction);
            Terminal.Session.AddAction(new ZeroAction(ActionType.MenuItem, "Configuración@Propiedades", OpenConfiguration));
            Terminal.Session.AddAction(new ZeroAction(ActionType.MenuItem, "Configuración@Usuarios", OpenUsers, "ValidateUser"));

            Terminal.Session.AddAction(new ZeroAction(ActionType.BackgroudAction, "terminalZeroValidation", IsTerminalZero));
            Terminal.Session.AddAction(new ZeroAction(ActionType.BackgroudAction, "userAuthorization", CanOpenConfiguration));
        }

        private void BuildRulesActions()
        {
            Terminal.Session.AddRule(new ZeroRule("ValidateUser", Resources.ValidUser, Resources.UnauthorizedUser, "userAuthorization"));
            Terminal.Session.AddRule(new ZeroRule("ValidateTerminalZero", Resources.ValidTerminal, Resources.UnauthorizedTrminal, "terminalZeroValidation"));
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Init()
        {
            StartSyncronizer();
        }

        private void StartSyncronizer()
        {
            Sync = new Synchronizer();
            double milsec = Sync.LoadConfiguration(Terminal, new ConfigurationEntities(), Terminal.Session);
            Terminal.Session.Notifier.SetUserMessage(false, string.Format("Sincronizando cada {0} minutos", (milsec / 1000) / 60));
            Sync.SyncStarting += new EventHandler<Synchronizer.SyncStartingEventArgs>(Sync_SyncStarting);
            string msg = "";
            ZeroModule module = this;
            if (Terminal.Manager.CanExecute(SyncAction, out msg))
                SyncAction.Execute(null);
            else
                Terminal.Session.Notifier.SendNotification(msg);
        }

        private void Sync_SyncStarting(object sender, Synchronizer.SyncStartingEventArgs e)
        {
            e.Notifier = Terminal.Session.Notifier;
            e.SyncService = Terminal.Session.GetParameter<ISyncService>().Value;
            e.FileTransferService = Terminal.Session.GetParameter<IFileTransfer>().Value;
            e.Modules = Terminal.Session.GetParameter<List<ZeroModule>>().Value;
        }

        private void StartSync(ZeroRule rule)
        {
            Sync.Start();
        }

        private void OpenUsers(ZeroRule rule)
        {
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = new Controls.Users() });
        }

        private void OpenConfiguration(ZeroRule rule)
        {
// ReSharper disable RedundantNameQualifier
            var P = new ZeroConfiguration.Controls.Properties(Terminal);
// ReSharper restore RedundantNameQualifier
            P.UpdateTimeRemaining(Sync);
            if (Terminal.Manager.ValidateRule("ValidateTerminalZero"))
                P.Mode = Mode.Update;

            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
        }

        private void CanOpenConfiguration(ZeroRule rule)
        {
            rule.Satisfied = false;
        }

        private void IsTerminalZero(ZeroRule rule)
        {
            if (Terminal != null && ConfigurationEntities.IsTerminalZero(new ConfigurationEntities(), Terminal.TerminalCode))
                rule.Satisfied = (TerminalStatus == ModuleStatus.Valid || TerminalStatus == ModuleStatus.NeedsSync);
            else
                rule.Satisfied = false;
        }

        #region ITerminalManager Members

        public event EventHandler ConfigurationRequired;

        private void OnConfigurationRequired()
        {
            if (ConfigurationRequired != null)
                ConfigurationRequired(this, EventArgs.Empty);
        }

        public ModuleStatus GetModuleStatus(ZeroModule module)
        {
            return ConfigurationEntities.GetTerminalModuleStatus(new ConfigurationEntities(), Terminal.TerminalCode, module);
        }

        public void InitializeTerminal()
        {
            using (ConfigurationEntities conf = new ConfigurationEntities())
            {
                if (conf.Terminals.FirstOrDefault(t => t.Code == Terminal.TerminalCode) == null)
                    ConfigurationEntities.AddNewTerminal(conf, Terminal.TerminalCode, Terminal.TerminalName);

                ConfigurationEntities.CreateTerminalProperties(conf, Terminal.TerminalCode);
            }

        }

        public bool ExecuteAction(ZeroAction action)
        {
            bool ret = false; 
            try
            {
                string result = "";
                if (Terminal.Manager.CanExecute(action, out result))
                {

                    action.Execute(null);
                    ret = true;
                }
                else
                {
                    Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Action {0} Error:", action.Name, result));
                    Terminal.Session.Notifier.SendNotification(string.Format(Resources.CannotExecuteAction + "\n\nProblemas: {0} ", result));
                }

            }
            catch (Exception ex)
            {
                Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, string.Format("Action {0} Error:", action.Name, ex.ToString()));
                Terminal.Session.Notifier.SendNotification(Resources.UnexpectedError + ".\n " + Resources.ContactSystemAdministrator);
            }

            return ret;
        }

        public bool ValidateRule(string ruleName)
        {
            string aux = "";
            return ValidateRule(ruleName, ref aux);
        }

        public bool ValidateRule(string ruleName, ref string result)
        {
            bool ret = false;
            if (Terminal.Session.SystemRules.ContainsKey(ruleName))
            {
                if (!Terminal.Session.SystemRules[ruleName].Satisfied.HasValue)
                    Terminal.Session.SystemRules[ruleName].Check();

                ret = Terminal.Session.SystemRules[ruleName].Satisfied.Value;
                result = Terminal.Session.SystemRules[ruleName].Result;

                if (Terminal.Session.Notifier != null)
                    Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Rule {0} - Status: {1}, result {2}", ruleName, ret, result));
            }
            else
            {
                if (Terminal.Session.Notifier != null)
                    Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Rule {0} does not exists", ruleName));
                result = "No existe la regla con el nombre ''" + ruleName + "''";
            }

            return ret;
        }

        public bool CanExecute(ZeroAction action, out string result)
        {
            bool ret = true;
            result = "";
            if (ret = ValidateActionParams(ref result, action))
            {
                if (!action.CanExecute(null))
                {
                    ret = false;
                    result = action.RuleToSatisfy.Result;
                }
            }

            return ret;
        }

        public bool ExistsAction(string actionName, out ZeroAction action)
        {
            action = null;
            if (Terminal.Session.SystemActions.ContainsKey(actionName))
            {
                action = Terminal.Session.SystemActions[actionName];
                return true;
            }
            return false;
        }

        private bool ValidateActionParams(ref string result, ZeroAction buttonAction)
        {
            bool ret = true;
            ZeroActionParameterBase obj = null;
            foreach (var item in buttonAction.Parameters)
            {
                if (Terminal.Session.SessionParams.ContainsKey(item.Name))
                    obj = Terminal.Session.SessionParams[item.Name];

                if ((obj == null || obj.Value == null) && item.IsMandatory)
                {
                    ret = false;
                    result += "No se ha asignado el parámetro '" + item.Name + "'\n";
                }

                obj = null;
            }

            if (ret && buttonAction.RuleToSatisfy != null && buttonAction.RuleToSatisfy.CheckRuleAction != null)
                ValidateActionParams(ref result, buttonAction.RuleToSatisfy.CheckRuleAction);

            return ret;
        }

        public List<ZeroAction> BuilSessionActions()
        {
            List<ZeroAction> validActions = new List<ZeroAction>();
            //El sistema posee reglas, estas reglas poseen (o no) una acción asociada para que valide la regla, las lineas siguentes
            //asocian a las reglas con sus respectias acciones
            foreach (var item in Terminal.Session.SystemRules)
            {
                item.Value.Satisfied = null;
                if (Terminal.Session.SystemActions.ContainsKey(item.Value.CheckRuleActionName))
                    item.Value.CheckRuleAction = Terminal.Session.SystemActions[item.Value.CheckRuleActionName];
                else
                    item.Value.Satisfied = false;
            }

            //despues de haber cargado las reglas, ahora asocio las acciones finales a las reglas a validar
            foreach (var item in Terminal.Session.SystemActions)
            {
                if (!string.IsNullOrEmpty(item.Value.RuleToSatisfyName))
                    item.Value.RuleToSatisfy = Terminal.Session.SystemRules[item.Value.RuleToSatisfyName];

                validActions.Add(item.Value);
            }


            return validActions;
        }

        public List<ZeroAction> GetShorcutActions()
        {
            var actions = new List<ZeroAction>();
            string[] ret = new string[] { };
            using (var conf = new ConfigurationEntities())
            {
                TerminalProperty prop = conf.TerminalProperties.FirstOrDefault(tp => tp.TerminalCode == Terminal.TerminalCode && tp.Code == Namespace.Properties.HomeShorcuts);
                if (prop != null)
                {
                    if (prop.LargeValue != null)
                    {
                        ret = prop.LargeValue.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        ZeroAction act = null;
                        string[] actParts;
                        foreach (var item in ret)
                        {
                            actParts = item.Split('|');
                            if (Terminal.Manager.ExistsAction(actParts[0].Trim(), out act))
                            {
                                if (actParts.Length > 1)
                                {
                                    act.Alias = actParts[1].Trim();
                                }
                                else
                                {
                                    act.Alias = actParts[0].Substring(actParts[0].LastIndexOf('@') + 1).Trim(); ;
                                }
                                actions.Add(act);
                            }
                        }
                    }
                }
            }
            return actions;
        }

        #endregion

        
    }
}
