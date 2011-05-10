using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Windows;
using ZeroBusiness;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration.Pages;
using ZeroConfiguration.Pages.Controls;
using ZeroConfiguration.Properties;
using ZeroGUI;
using ActionParameters = ZeroCommonClasses.GlobalObjects.ActionParameters;

namespace ZeroConfiguration
{
    public class ZeroConfigurationModule : ZeroCommonClasses.ZeroModule, ITerminalManager
    {
        private const string K_administrator = "Administrator";
        private const string K_password = "admin";

        private Synchronizer _sync;
        public ZeroAction SyncAction;
        private bool _isTerminalZero;
        public ZeroConfigurationModule()
            : base(2, Resources.ConfigurationModuleDescription)
        {
            BuildPosibleActions();
            BuildRulesActions();
            ZeroCommonClasses.Terminal.Instance.Manager = this;
        }

        private void BuildPosibleActions()
        {
            //actions.Add(new ZeroAction(ActionType.MenuItem, "Reload", (rule) => { OnConfigurationRequired(); }));
            
            SyncAction = new ZeroAction( ActionType.BackgroudAction, Actions.ExecSync, StartSync);
            SyncAction.Parameters.Add(new ZeroActionParameterBase(typeof(ISyncService), true));
            SyncAction.Parameters.Add(new ZeroActionParameterBase(ActionParameters.Modules, true));
            ZeroCommonClasses.Terminal.Instance.Session.AddAction(SyncAction);
            ZeroCommonClasses.Terminal.Instance.Session.AddAction(new ZeroAction(ActionType.MenuItem, Actions.OpenPropertiesView, OpenConfiguration));
            ZeroCommonClasses.Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenUserListView, OpenUsers, Rules.IsValidUser));
            var changePassAction = new ZeroAction( ActionType.MenuItem, Actions.OpenUserPasswordChangeMessage, OpenChangePassword);
            changePassAction.Parameters.Add(new ZeroActionParameterBase(typeof (MembershipUser), true));
            ZeroCommonClasses.Terminal.Instance.Session.AddAction(changePassAction);
        }

        private void BuildRulesActions()
        {   
            ZeroCommonClasses.Terminal.Instance.Session.AddRule(Rules.IsValidUser, CanOpenConfiguration);
            ZeroCommonClasses.Terminal.Instance.Session.AddRule(Rules.IsTerminalZero, IsTerminalZero);
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
            if (Membership.GetUser(K_administrator) == null)
            {
                Membership.CreateUser(K_administrator, K_password);
            }
        }

        private void OpenLogInDialog()
        {
#if DEBUG
            ZeroCommonClasses.Terminal.Instance.Session.AddNavigationParameter(new ZeroActionParameter<MembershipUser>(false, Membership.GetUser(K_administrator, true), false));
#else
            var view = new UserLogIn();
            bool? dialogResult = ZeroMessageBox.Show(view, Resources.LogIn,ResizeMode.NoResize);
            if (dialogResult.GetValueOrDefault())
            {
                if (Membership.ValidateUser(view.UserName, view.UserPass))
                {
                    ZeroCommonClasses.Terminal.Instance.Session.AddNavigationParameter(new ZeroActionParameter<MembershipUser>(false,Membership.GetUser(view.UserName,true), false));
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
#endif
        }

        private void StartSyncronizer()
        {
            _sync = new Synchronizer();
            double milsec;
            using (var conf = new ConfigurationModelManager())
            {
                milsec = _sync.LoadConfiguration(conf, ZeroCommonClasses.Terminal.Instance.Session);    
            }

            ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false, string.Format(Resources.SyncEveryFormat, (milsec / 1000) / 60));
            _sync.SyncStarting += SyncSyncStarting;
            _sync.SyncFinished += _sync_SyncFinished;
            
            var sb = new StringBuilder();
            if (SyncAction.CanExecute(sb))
                SyncAction.Execute(null);
            else
                ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.SendNotification(sb.ToString());
        }

        void _sync_SyncFinished(object sender, EventArgs e)
        {
            using (var conf = new ConfigurationModelManager())
            {
                _isTerminalZero = ConfigurationModelManager.IsTerminalZero(conf, ZeroCommonClasses.Terminal.Instance.TerminalCode);
            }
            OnConfigurationRequired();
        }

        private void SyncSyncStarting(object sender, Synchronizer.SyncStartingEventArgs e)
        {
            e.Notifier = ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier;
            e.SyncService = ZeroCommonClasses.Terminal.Instance.Session.GetParameter<ISyncService>().Value;
            e.FileTransferService = ZeroCommonClasses.Terminal.Instance.Session.GetParameter<IFileTransfer>().Value;
            e.Modules = ZeroCommonClasses.Terminal.Instance.CurrentClient.ModuleList;
        }

        private void StartSync()
        {
            _sync.Start();
        }

        private void OpenUsers()
        {
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = new Users() });
        }

        private void OpenChangePassword()
        {
            var pswChange = new UserChangePassword();
            pswChange.DataContext = ZeroCommonClasses.Terminal.Instance.Session.GetParameter<MembershipUser>().Value;
            ZeroMessageBox.Show(pswChange, Resources.ChangePassword, ResizeMode.NoResize);
        }

        private void OpenConfiguration()
        {
            // ReSharper disable RedundantNameQualifier
            var P = new Pages.Properties();
            // ReSharper restore RedundantNameQualifier
            P.UpdateTimeRemaining(_sync);
            if (ZeroCommonClasses.Terminal.Instance.Manager.ValidateRule(Rules.IsTerminalZero))
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
            if (_isTerminalZero)
                ret = (TerminalStatus == ZeroCommonClasses.ModuleStatus.Valid || TerminalStatus == ZeroCommonClasses.ModuleStatus.NeedsSync);

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

        public ZeroCommonClasses.ModuleStatus GetModuleStatus(ZeroCommonClasses.ZeroModule module)
        {
            return ConfigurationModelManager.GetTerminalModuleStatus(new ConfigurationModelManager(), ZeroCommonClasses.Terminal.Instance.TerminalCode, module);
        }

        public void InitializeTerminal()
        {
            using (var conf = new ConfigurationModelManager())
            {
                Terminal T = conf.Terminals.FirstOrDefault(t => t.Code == ZeroCommonClasses.Terminal.Instance.TerminalCode);
                if (T == null)
                {
                    Terminal.AddNewTerminal(conf, ZeroCommonClasses.Terminal.Instance.TerminalCode, ZeroCommonClasses.Terminal.Instance.TerminalName);
                }
                else 
                {
                    _isTerminalZero = T.IsTerminalZero;
                    if(ZeroCommonClasses.Terminal.Instance.TerminalCode == 0 && !T.IsTerminalZero)
                    {
                        T.IsTerminalZero = true;
                        conf.SaveChanges();
                    }
                }
                ConfigurationModelManager.CreateTerminalProperties(conf,ZeroCommonClasses.Terminal.Instance.TerminalCode);
            }

        }

        public bool ValidateRule(string ruleName)
        {
            return ZeroCommonClasses.Terminal.Instance.Session.SystemRules.ContainsKey(ruleName) &&
                   ZeroCommonClasses.Terminal.Instance.Session.SystemRules[ruleName].Invoke(null);
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
                    ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Verbose, string.Format("Action {0} Error: {1}", action.Name, result));
                    ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.SendNotification(string.Format(Resources.CannotExecuteAction + "\n\nProblemas: {0} ", result));
                }

            }
            catch (Exception ex)
            {
                ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, string.Format("Action {0} Error: {1}", action.Name, ex));
                ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.SendNotification(Resources.UnexpectedError + ".\n " + Resources.ContactSystemAdministrator);
            }

            return ret;
        }

        public bool ExistsAction(string actionName, out ZeroAction action)
        {
            action = null;
            if (ZeroCommonClasses.Terminal.Instance.Session.SystemActions.ContainsKey(actionName))
            {
                action = ZeroCommonClasses.Terminal.Instance.Session.SystemActions[actionName];
                return true;
            }
            Trace.WriteLineIf(ConfigurationContext.LogLevel.TraceWarning, string.Format("Action {0} is missing!", actionName));
            return false;
        }
        
        public List<ZeroAction> BuilSessionActions()
        {
            var validActions = new List<ZeroAction>();
            //El sistema posee reglas, estas reglas poseen (o no) una acción asociada para que valide la regla, las lineas siguentes
            //asocian a las reglas con sus respectias acciones

            //despues de haber cargado las reglas, ahora asocio las acciones finales a las reglas a validar
            foreach (var item in ZeroCommonClasses.Terminal.Instance.Session.SystemActions)
            {
                if (!string.IsNullOrEmpty(item.Value.RuleToSatisfyName))
                {
                    if (ZeroCommonClasses.Terminal.Instance.Session.SystemRules.ContainsKey(item.Value.RuleToSatisfyName))
                    {
                        item.Value.RuleToSatisfy = ZeroCommonClasses.Terminal.Instance.Session.SystemRules[item.Value.RuleToSatisfyName];
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
            using (var conf = new ConfigurationModelManager())
            {
                TerminalProperty prop = conf.TerminalProperties.FirstOrDefault(tp => tp.TerminalCode == ZeroCommonClasses.Terminal.Instance.TerminalCode && tp.Code == SystemProperty.HomeShortcut.Code);
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
                            if (ZeroCommonClasses.Terminal.Instance.Manager.ExistsAction(actParts[0].Trim(), out act))
                            {
                                act.SetAlias(actParts);
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
