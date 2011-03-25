using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Windows;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration.Entities;
using ZeroConfiguration.Pages;
using ZeroConfiguration.Pages.Controls;
using ZeroConfiguration.Properties;
using ZeroGUI;

namespace ZeroConfiguration
{
    public class ZeroConfigurationModule : ZeroModule, ITerminalManager
    {
        private Synchronizer _sync;
        public ZeroAction SyncAction;
        private bool _isTerminalZero;
        public ZeroConfigurationModule(ITerminal iCurrentTerminal)
            : base(iCurrentTerminal, 2, "Administra configuraciones sobre las terminales")
        {
            BuildPosibleActions();
            BuildRulesActions();
        }

        private void BuildPosibleActions()
        {
            //actions.Add(new ZeroAction(ActionType.MenuItem, "Reload", (rule) => { OnConfigurationRequired(); }));
            
            SyncAction = new ZeroAction( ActionType.BackgroudAction, Actions.ExecSync, StartSync);
            SyncAction.Parameters.Add(new ZeroActionParameterBase(typeof(ISyncService), true));
            SyncAction.Parameters.Add(new ZeroActionParameterBase(ActionParameters.Modules, true));
            OwnerTerminal.Session.AddAction(SyncAction);
            OwnerTerminal.Session.AddAction(new ZeroAction(ActionType.MenuItem, Actions.OpenPropertiesView, OpenConfiguration));
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenUserListView, OpenUsers, Rules.IsValidUser));
            var changePassAction = new ZeroAction( ActionType.MenuItem, Actions.OpenUserPasswordChangeMessage, OpenChangePassword);
            changePassAction.Parameters.Add(new ZeroActionParameterBase(typeof (MembershipUser), true));
            OwnerTerminal.Session.AddAction(changePassAction);
        }

        private void BuildRulesActions()
        {   
            OwnerTerminal.Session.AddRule(Rules.IsValidUser, CanOpenConfiguration);
            OwnerTerminal.Session.AddRule(Rules.IsTerminalZero, IsTerminalZero);
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
//#if DEBUG
//            OwnerTerminal.Session.AddNavigationParameter(new ZeroActionParameter<MembershipUser>(false, Membership.GetUser("Administrator", true), false));
//#else
            var view = new UserLogIn();
            bool? dialogResult = ZeroMessageBox.Show(view, Resources.LogIn,ResizeMode.NoResize);
            if (dialogResult.GetValueOrDefault())
            {
                if (Membership.ValidateUser(view.UserName, view.UserPass))
                {
                    OwnerTerminal.Session.AddNavigationParameter(new ZeroActionParameter<MembershipUser>(false,Membership.GetUser(view.UserName,true), false));
                }
                else
                {
                    ZeroMessageBox.Show(Resources.MsgIncorrectUserPassTryAgain, Resources.Fail, ResizeMode.NoResize);
                    OpenLogInDialog();
                }
            }
            else
            {
                ZeroMessageBox.Show(Resources.MsgLogInPlease+"\nEl sistema se cerrara.", Resources.Fail, ResizeMode.NoResize, MessageBoxButton.OK);
                ZeroAction action;
                if(ExistsAction(Actions.AppExit,out action))
                {
                    ExecuteAction(action);
                }
            }
//#endif
        }

        private void StartSyncronizer()
        {
            _sync = new Synchronizer();
            double milsec;
            using (var conf = new ConfigurationEntities())
            {
                milsec = _sync.LoadConfiguration(OwnerTerminal, conf, OwnerTerminal.Session);    
            }

            OwnerTerminal.Session.Notifier.SetUserMessage(false, string.Format(Resources.SyncEveryFormat, (milsec / 1000) / 60));
            _sync.SyncStarting += SyncSyncStarting;
            _sync.SyncFinished += _sync_SyncFinished;
            
            var sb = new StringBuilder();
            if (SyncAction.CanExecute(sb))
                SyncAction.Execute(null);
            else
                OwnerTerminal.Session.Notifier.SendNotification(sb.ToString());
        }

        void _sync_SyncFinished(object sender, EventArgs e)
        {
            using (var conf = new ConfigurationEntities())
            {
                _isTerminalZero = ConfigurationEntities.IsTerminalZero(conf, OwnerTerminal.TerminalCode);
            }
            OnConfigurationRequired();
        }

        private void SyncSyncStarting(object sender, Synchronizer.SyncStartingEventArgs e)
        {
            e.Notifier = OwnerTerminal.Session.Notifier;
            e.SyncService = OwnerTerminal.Session.GetParameter<ISyncService>().Value;
            e.FileTransferService = OwnerTerminal.Session.GetParameter<IFileTransfer>().Value;
            e.Modules = OwnerTerminal.Session.GetParameter<List<ZeroModule>>().Value;
        }

        private void StartSync()
        {
            _sync.Start();
        }

        private void OpenUsers()
        {
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = new Users(OwnerTerminal) });
        }

        private void OpenChangePassword()
        {
            var pswChange = new UserChangePassword();
            pswChange.DataContext = OwnerTerminal.Session.GetParameter<MembershipUser>().Value;
            ZeroMessageBox.Show(pswChange, Resources.ChangePassword, ResizeMode.NoResize);
        }

        private void OpenConfiguration()
        {
            // ReSharper disable RedundantNameQualifier
            var P = new Pages.Properties(OwnerTerminal);
            // ReSharper restore RedundantNameQualifier
            P.UpdateTimeRemaining(_sync);
            if (OwnerTerminal.Manager.ValidateRule(Rules.IsTerminalZero))
                P.ControlMode = ControlMode.Update;

            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
        }

        private bool CanOpenConfiguration(object param)
        {
            bool ret = true;
            
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
            if (OwnerTerminal != null && _isTerminalZero)
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
            return ConfigurationEntities.GetTerminalModuleStatus(new ConfigurationEntities(), OwnerTerminal.TerminalCode, module);
        }

        public void InitializeTerminal()
        {
            using (var conf = new ConfigurationEntities())
            {
                Terminal T = conf.Terminals.FirstOrDefault(t => t.Code == OwnerTerminal.TerminalCode);
                if (T == null)
                {
                    Terminal.AddNewTerminal(conf, OwnerTerminal.TerminalCode, OwnerTerminal.TerminalName);
                }
                else 
                {
                    _isTerminalZero = T.IsTerminalZero;
                    if(OwnerTerminal.TerminalCode == 0 && !T.IsTerminalZero)
                    {
                        T.IsTerminalZero = true;
                        conf.SaveChanges();
                    }
                }
                ConfigurationEntities.CreateTerminalProperties(conf, OwnerTerminal.TerminalCode);
            }

        }

        public bool ValidateRule(string ruleName)
        {
            return OwnerTerminal.Session.SystemRules.ContainsKey(ruleName) &&
                   OwnerTerminal.Session.SystemRules[ruleName].Invoke(null);
        }

        public bool ExecuteAction(ZeroAction action)
        {
            bool ret = false;
            try
            {
                var result = new StringBuilder();
                if (action.CanExecute(result))
                {
                    action.Execute(null);
                    ret = true;
                }
                else
                {
                    OwnerTerminal.Session.Notifier.Log(TraceLevel.Verbose, string.Format("Action {0} Error: {1}", action.Name, result));
                    OwnerTerminal.Session.Notifier.SendNotification(string.Format(Resources.CannotExecuteAction + "\n\nProblemas: {0} ", result));
                }

            }
            catch (Exception ex)
            {
                OwnerTerminal.Session.Notifier.Log(TraceLevel.Error, string.Format("Action {0} Error: {1}", action.Name, ex));
                OwnerTerminal.Session.Notifier.SendNotification(Resources.UnexpectedError + ".\n " + Resources.ContactSystemAdministrator);
            }

            return ret;
        }

        public bool ExistsAction(string actionName, out ZeroAction action)
        {
            action = null;
            if (OwnerTerminal.Session.SystemActions.ContainsKey(actionName))
            {
                action = OwnerTerminal.Session.SystemActions[actionName];
                return true;
            }
            Trace.WriteLineIf(ContextInfo.LogLevel.TraceWarning, string.Format("Action {0} is missing!", actionName));
            return false;
        }
        
        public List<ZeroAction> BuilSessionActions()
        {
            var validActions = new List<ZeroAction>();
            //El sistema posee reglas, estas reglas poseen (o no) una acción asociada para que valide la regla, las lineas siguentes
            //asocian a las reglas con sus respectias acciones

            //despues de haber cargado las reglas, ahora asocio las acciones finales a las reglas a validar
            foreach (var item in OwnerTerminal.Session.SystemActions)
            {
                if (!string.IsNullOrEmpty(item.Value.RuleToSatisfyName))
                {
                    if (OwnerTerminal.Session.SystemRules.ContainsKey(item.Value.RuleToSatisfyName))
                    {
                        item.Value.RuleToSatisfy = OwnerTerminal.Session.SystemRules[item.Value.RuleToSatisfyName];
                    }
                }

                validActions.Add(item.Value);
            }


            return validActions;
        }

        public List<ZeroAction> GetShorcutActions()
        {
            var actions = new List<ZeroAction>();
            var ret = new string[] { };
            using (var conf = new ConfigurationEntities())
            {
                TerminalProperty prop = conf.TerminalProperties.FirstOrDefault(tp => tp.TerminalCode == OwnerTerminal.TerminalCode && tp.Code == Namespace.Properties.HomeShorcuts);
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
                            if (OwnerTerminal.Manager.ExistsAction(actParts[0].Trim(), out act))
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
