using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses;
using System.Windows.Controls;
using ZeroConfiguration.Entities;
using ZeroConfiguration.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.GlobalObjects;


namespace ZeroConfiguration
{
    public partial class ZeroConfigurationModule : ZeroModule, ITerminalManager
    {
        private Synchronizer Sync = null;
        public ZeroAction SyncAction = null;

        public ZeroConfigurationModule(ITerminal iCurrentTerminal)
            : base(iCurrentTerminal, 2, "Administra configuraciones sobre las terminales")
        {
            
        }

        public override void BuildPosibleActions(List<ZeroAction> actions)
        {
            //actions.Add(new ZeroAction(ActionType.MenuItem, "Reload", (rule) => { OnConfigurationRequired(); }));

            SyncAction = new ZeroAction(ActionType.BackgroudAction, "Configuración@Sincronizar", StartSync);
            SyncAction.Parameters.Add(new ZeroActionParameterBase(typeof(ISyncService), true));
            SyncAction.Parameters.Add(new ZeroActionParameterBase("ExistingModules", true));
            actions.Add(SyncAction);
            actions.Add(new ZeroAction(ActionType.MenuItem, "Configuración@Propiedades", OpenConfiguration));
            actions.Add(new ZeroAction(ActionType.MenuItem, "Configuración@Usuarios", OpenUsers, "ValidateUser"));

            actions.Add(new ZeroAction(ActionType.BackgroudAction, "terminalZeroValidation", isTerminalZero));
            actions.Add(new ZeroAction(ActionType.BackgroudAction, "userAuthorization", CanOpenConfiguration));
        }

        public override void BuildRulesActions(List<ZeroRule> rules)
        {
            rules.Add(new ZeroRule("ValidateUser", "Usuario válido", "No esta autorizado para realizar esta acción!", "userAuthorization"));
            rules.Add(new ZeroRule("ValidateTerminalZero", "Terminal válida", "Esta Terminal no esta autorizada para realizar esta acción!", "terminalZeroValidation"));
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
            Terminal.Session.Notifier.SetUserMessage(false,string.Format("Sincronizando cada {0} minutos",(milsec/1000)/60));
            Sync.SyncStarting += new EventHandler<Synchronizer.SyncStartingEventArgs>(Sync_SyncStarting);
            string msg = "";
            ZeroModule module = this;
            if (Terminal.Session.CanExecute(SyncAction, out msg))
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
            Properties P = new Properties(Terminal);
            P.UpdateTimeRemaining(Sync);
            if (Terminal.Session.ValidateRule("ValidateTerminalZero"))
                P.Mode = Mode.Update;

            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
        }

        private void CanOpenConfiguration(ZeroRule rule)
        {
            rule.Satisfied = false;
        }

        private void isTerminalZero(ZeroRule rule)
        {
            if (Terminal != null && ConfigurationEntities.IsTerminalZero(new ConfigurationEntities(), Terminal.TerminalCode))
                rule.Satisfied = (TerminalStatus == ModuleStatus.Valid || TerminalStatus == ModuleStatus.NeedsSync);
            else
                rule.Satisfied = false;
        }

        #region ITerminalClientManager Members

        public event EventHandler ConfigurationRequired;

        private void OnConfigurationRequired()
        {
            if (ConfigurationRequired != null)
                ConfigurationRequired(this, EventArgs.Empty);
        }

        public ModuleStatus GetModuleStatus(ZeroModule c)
        {
            return ConfigurationEntities.GetTerminalModuleStatus(new ConfigurationEntities(), Terminal.TerminalCode,c);
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

        public bool Navigate(out string result, ZeroAction Action)
        {
            bool ret = true;
            result = "";
            if (Terminal.Session.CanExecute(Action, out result))
            {
                try
                {
                    Action.Execute(null);
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                    ret = false;
                }

            }
            else
            {
                ret = false;
            }

            return ret;
        }

        public List<ZeroAction> GetShorcutActions()
        {
            List<ZeroAction> actions = new List<ZeroAction>();
            string[] ret = new string[] { };
            using (ConfigurationEntities conf = new ConfigurationEntities())
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
                            if (Terminal.Session.ExistsAction(actParts[0].Trim(), out act))
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
