using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration.Entities;
using ZeroConfiguration.Pages;
using ZeroConfiguration.Properties;
using System.Web.Security;
using System.Text;


namespace ZeroConfiguration
{
    public partial class ZeroConfigurationModule : ZeroModule, ITerminalManager
    {
        private Synchronizer _sync;
        public ZeroAction SyncAction;

        public ZeroConfigurationModule(ITerminal iCurrentTerminal)
            : base(iCurrentTerminal, 2, "Administra configuraciones sobre las terminales")
        {
            BuildPosibleActions();
            BuildRulesActions();
        }

        private void BuildPosibleActions()
        {
            //actions.Add(new ZeroAction(ActionType.MenuItem, "Reload", (rule) => { OnConfigurationRequired(); }));
            
            SyncAction = new ZeroAction(Terminal.Session, ActionType.BackgroudAction, "Configuración@Sincronizar", StartSync);
            SyncAction.Parameters.Add(new ZeroActionParameterBase(typeof(ISyncService), true));
            SyncAction.Parameters.Add(new ZeroActionParameterBase("ExistingModules", true));
            Terminal.Session.AddAction(SyncAction);
            Terminal.Session.AddAction(new ZeroAction(Terminal.Session,ActionType.MenuItem, "Configuración@Propiedades", OpenConfiguration));
            Terminal.Session.AddAction(new ZeroAction(Terminal.Session, ActionType.MenuItem, "Configuración@Usuarios", OpenUsers, "ValidateUser"));
        }

        private void BuildRulesActions()
        {
            Terminal.Session.AddRule("ValidateUser", CanOpenConfiguration);
            Terminal.Session.AddRule("ValidateTerminalZero", IsTerminalZero);
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Init()
        {
            StartSyncronizer();
            ValidateAdminUser();
            OpenLogInDialog();
        }

        private void ValidateAdminUser()
        {
            if (Membership.GetUser("Administrator") == null)
            {
                Membership.CreateUser("Administrator", "tzadmin");
            }
        }

        private void OpenLogInDialog()
        {
#if DEBUG
            Terminal.Session.AddNavigationParameter(new ZeroActionParameter<MembershipUser>(false, Membership.GetUser("Administrator", true), false));
#else
            var obj = new ZeroGUI.ZeroMessageBox(true);
            var view = new UserLogIn();
            obj.Content = view;
            obj.Title = "Log In";
            obj.AllowsTransparency = true;
            obj.BorderThickness= new Thickness(1);
            obj.BorderBrush = Brushes.Black;
            obj.WindowStyle = WindowStyle.None;
            obj.SizeToContent = SizeToContent.WidthAndHeight;
            obj.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            bool? dialogResult = obj.ShowDialog();
            if (dialogResult.GetValueOrDefault())
            {
                if (Membership.ValidateUser(view.UserName, view.UserPass))
                {
                    Terminal.Session.AddNavigationParameter(new ZeroActionParameter<MembershipUser>(false,Membership.GetUser(view.UserName,true), false));
                    ZeroGUI.ZeroMessageBox.Show(view.UserName, Resources.Welcome);
                }
                else
                {
                    ZeroGUI.ZeroMessageBox.Show(Resources.MsgIncorrectUserPassTryAgain, Resources.Fail);
                    OpenLogInDialog(null);
                }
            }
            else
            {
                ZeroGUI.ZeroMessageBox.Show(Resources.MsgLogInPlease, Resources.Fail);
                OpenLogInDialog(null);
            }
#endif
        }

        private void StartSyncronizer()
        {
            _sync = new Synchronizer();
            double milsec = _sync.LoadConfiguration(Terminal, new ConfigurationEntities(), Terminal.Session);
            Terminal.Session.Notifier.SetUserMessage(false, string.Format(Resources.SyncEveryFormat, (milsec / 1000) / 60));
            _sync.SyncStarting += SyncSyncStarting;
            
            StringBuilder sb = new StringBuilder();
            if (SyncAction.CanExecute(sb))
                SyncAction.Execute(null);
            else
                Terminal.Session.Notifier.SendNotification(sb.ToString());
        }

        private void SyncSyncStarting(object sender, Synchronizer.SyncStartingEventArgs e)
        {
            e.Notifier = Terminal.Session.Notifier;
            e.SyncService = Terminal.Session.GetParameter<ISyncService>().Value;
            e.FileTransferService = Terminal.Session.GetParameter<IFileTransfer>().Value;
            e.Modules = Terminal.Session.GetParameter<List<ZeroModule>>().Value;
        }

        private void StartSync()
        {
            _sync.Start();
        }

        private void OpenUsers()
        {
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = new Users() });
        }

        private void OpenConfiguration()
        {
            // ReSharper disable RedundantNameQualifier
            var P = new ZeroConfiguration.Pages.Properties(Terminal);
            // ReSharper restore RedundantNameQualifier
            P.UpdateTimeRemaining(_sync);
            if (Terminal.Manager.ValidateRule("ValidateTerminalZero"))
                P.Mode = Mode.Update;

            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
        }

        private bool CanOpenConfiguration(object param)
        {
            bool ret = false;
            if (param != null)
                if (param is StringBuilder)
                {
                    ((StringBuilder)param).AppendLine(ret ? Resources.ValidUser : Resources.UnauthorizedUser);
                }

            return ret;
        }

        private bool IsTerminalZero(object param)
        {
            bool ret = false;
            if (Terminal != null && ConfigurationEntities.IsTerminalZero(new ConfigurationEntities(), Terminal.TerminalCode))
                ret = (TerminalStatus == ModuleStatus.Valid || TerminalStatus == ModuleStatus.NeedsSync);

            if (param != null)
                if (param is StringBuilder)
                {
                    ((StringBuilder)param).AppendLine(ret ? Resources.ValidTerminal : Resources.UnauthorizedTrminal);
                }


            return ret;
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

        public bool ValidateRule(string ruleName)
        {
            return Terminal.Session.SystemRules.ContainsKey(ruleName) &&
                   Terminal.Session.SystemRules[ruleName].Invoke(null);
        }

        public bool ExecuteAction(ZeroAction action)
        {
            bool ret = false;
            try
            {
                StringBuilder result = new StringBuilder();
                if (action.CanExecute(result))
                {
                    action.Execute(null);
                    ret = true;
                }
                else
                {
                    Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Action {0} Error: {1}", action.Name, result));
                    Terminal.Session.Notifier.SendNotification(string.Format(Resources.CannotExecuteAction + "\n\nProblemas: {0} ", result));
                }

            }
            catch (Exception ex)
            {
                Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, string.Format("Action {0} Error: {1}", action.Name, ex));
                Terminal.Session.Notifier.SendNotification(Resources.UnexpectedError + ".\n " + Resources.ContactSystemAdministrator);
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
        
        public List<ZeroAction> BuilSessionActions()
        {
            List<ZeroAction> validActions = new List<ZeroAction>();
            //El sistema posee reglas, estas reglas poseen (o no) una acción asociada para que valide la regla, las lineas siguentes
            //asocian a las reglas con sus respectias acciones

            //despues de haber cargado las reglas, ahora asocio las acciones finales a las reglas a validar
            foreach (var item in Terminal.Session.SystemActions)
            {
                if (!string.IsNullOrEmpty(item.Value.RuleToSatisfyName))
                {
                    if (Terminal.Session.SystemRules.ContainsKey(item.Value.RuleToSatisfyName))
                    {
                        item.Value.RuleToSatisfy = Terminal.Session.SystemRules[item.Value.RuleToSatisfyName];
                    }
                }

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
