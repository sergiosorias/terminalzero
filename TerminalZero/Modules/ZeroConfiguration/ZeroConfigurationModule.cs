using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ZeroBusiness;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration.Pages;
using ZeroConfiguration.Pages.Controls;
using ZeroConfiguration.Presentantion;
using ZeroConfiguration.Properties;
using ZeroGUI;
using Terminal = ZeroCommonClasses.Terminal;

namespace ZeroConfiguration
{
    public class ZeroConfigurationModule : ZeroModule, ITerminalManager
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
            Terminal.Instance.Manager = this;
        }

        private void BuildPosibleActions()
        {
            //actions.Add(new ZeroAction(ActionType.MenuItem, "Reload", (rule) => { OnConfigurationRequired(); }));

            SyncAction = new ZeroBackgroundAction(Actions.ExecSync, StartSync, null, false, false);
            SyncAction.AddParam(typeof(ISyncService), true);
            Terminal.Instance.Session.Actions.Add(SyncAction);
            Terminal.Instance.Session.Actions.Add(new ZeroBackgroundAction(Actions.AppHome, OpenHomePage, null, false));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenPropertiesView, OpenConfiguration, null, true));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenUserListView, OpenUsers, Rules.IsValidUser, true));
            var changePassAction = new ZeroAction(Actions.OpenUserPasswordChangeMessage, OpenChangePassword, null, true);
            changePassAction.AddParam(typeof(User), true);
            Terminal.Instance.Session.Actions.Add(changePassAction);
        }

        private void BuildRulesActions()
        {
            Terminal.Instance.Session.Rules.Add(Rules.IsValidUser, CanOpenConfiguration);
            Terminal.Instance.Session.Rules.Add(Rules.IsTerminalZero, IsTerminalZero);
        }

        public override void Initialize()
        {
            StartSyncronizer();
            ValidateAdminUser();
            Terminal.Instance.CurrentClient.Loaded += (o, e) => { OpenLogInDialog(); };
        }

        private void ValidateAdminUser()
        {
            if (User.GetUser(K_administrator) == null)
            {
                User.CreateUser(K_administrator, K_password);
            }
        }

        private void OpenLogInDialog()
        {
//#if DEBUG
//            ActionParameterBase userpParam = new ActionParameter<User>(false, User.GetUser(K_administrator, true), false);
//            Terminal.Instance.Session[userpParam.Name] = userpParam;
//#else
            var view = new UserLogIn();
            Terminal.Instance.CurrentClient.ShowDialog(view, dialogResult =>
            {
                if (dialogResult)
                {
                    if (User.ValidateUser(view.UserName, view.UserPass))
                    {
                        ActionParameterBase userpParam = new ActionParameter<User>(false, User.GetUser(view.UserName, true), false);
                        Terminal.Instance.Session[typeof(User)] = userpParam;
                        OpenHomePage(null);
                    }
                    else
                    {
                        Terminal.Instance.CurrentClient.ShowDialog(Resources.MsgIncorrectUserPassTryAgain, (res) => { OpenLogInDialog(); }, MessageBoxButton.OK);
                    }
                }
                else
                {
                    Terminal.Instance.CurrentClient.ShowDialog(Resources.MsgLogInPlease + "\nEl sistema se cerrara.",(res)=>
                    {
                        if (Terminal.Instance.Session.Actions[Actions.AppExit] != null)
                        {
                            Terminal.Instance.Session.Actions[Actions.AppExit].TryExecute();
                        }                                                                                                         
                    }, MessageBoxButton.OK);
}
            });
//#endif
        }

        private void StartSyncronizer()
        {
            _sync = new Synchronizer();
            double milsec;
            using (var conf = new ConfigurationModelManager())
            {
                milsec = _sync.LoadConfiguration(conf, Terminal.Instance.Session);
            }

            Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false, string.Format(Resources.SyncEveryFormat, (milsec / 1000) / 60));
            _sync.SyncStarting += SyncSyncStarting;
            _sync.SyncFinished += _sync_SyncFinished;

            SyncAction.TryExecute();
        }

        private void _sync_SyncFinished(object sender, EventArgs e)
        {
            using (var conf = new ConfigurationModelManager())
            {
                _isTerminalZero = ConfigurationModelManager.IsTerminalZero(conf, Terminal.Instance.TerminalCode);
            }
            OnConfigurationRequired();
        }

        private void SyncSyncStarting(object sender, Synchronizer.SyncStartingEventArgs e)
        {
            e.Notifier = Terminal.Instance.CurrentClient.Notifier;
            e.SyncService = (ISyncService)Terminal.Instance.Session[typeof(ISyncService)].Value;
            e.FileTransferService = (IFileTransfer)Terminal.Instance.Session[typeof(IFileTransfer)].Value;
            e.Modules = Terminal.Instance.CurrentClient.ModuleList;
        }

        private void StartSync(object parameter)
        {
            _sync.Start();
        }

        private void OpenHomePage(object obj)
        {
            Terminal.Instance.CurrentClient.ShowView(new HomePage());
        }

        private void OpenUsers(object parameter)
        {
            Terminal.Instance.CurrentClient.ShowView(new Users());
        }

        private void OpenChangePassword(object parameter)
        {
            var pswChange = new UserChangePassword();
            pswChange.DataContext = Terminal.Instance.Session[typeof(User)].Value;
            ZeroMessageBox.Show(pswChange, Resources.ChangePassword, ResizeMode.NoResize, MessageBoxButton.OKCancel);
        }

        private void OpenConfiguration(object parameter)
        {
            // ReSharper disable RedundantNameQualifier
            var view = new Pages.Properties();
            // ReSharper restore RedundantNameQualifier
            view.UpdateTimeRemaining(_sync);
            if (Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
                view.ControlMode = ControlMode.Update;
            else
                view.ControlMode = ControlMode.ReadOnly;

            Terminal.Instance.CurrentClient.ShowView(view);
        }

        private bool CanOpenConfiguration(object param)
        {
            return true;
        }

        private bool IsTerminalZero(object param)
        {
            bool ret = false;
            if (_isTerminalZero)
                ret = (TerminalStatus == ModuleStatus.Valid || TerminalStatus == ModuleStatus.NeedsSync);

            return ret;
        }

        #region ITerminalManager Members

        public event EventHandler ConfigurationRequired;

        private void OnConfigurationRequired()
        {
            if (ConfigurationRequired != null)
                ConfigurationRequired(this, EventArgs.Empty);
        }

        private ModuleStatus GetModuleStatus(ZeroModule module)
        {
            return ConfigurationModelManager.GetTerminalModuleStatus(new ConfigurationModelManager(), Terminal.Instance.TerminalCode, module);
        }

        public object GetMainViewModel()
        {
            return new MainViewModel();
        }

        public bool InitializeTerminal()
        {
            bool initialized = false;
            using (var conf = new ConfigurationModelManager())
            {
                ZeroBusiness.Entities.Configuration.Terminal T = conf.Terminals.FirstOrDefault(t => t.Code == Terminal.Instance.TerminalCode);
                if (T == null)
                {
                    ZeroBusiness.Entities.Configuration.Terminal.AddNewTerminal(conf, Terminal.Instance.TerminalCode, Terminal.Instance.TerminalName);
                }
                else
                {
                    _isTerminalZero = T.IsTerminalZero;
                    if (Terminal.Instance.TerminalCode == 0 && !T.IsTerminalZero)
                    {
                        T.IsTerminalZero = true;
                        conf.SaveChanges();
                    }
                }
                ConfigurationModelManager.CreateTerminalProperties(conf, Terminal.Instance.TerminalCode);

                Terminal.Instance.CurrentClient.ModuleList.ForEach(c =>
                {
                    c.TerminalStatus = GetModuleStatus(c);
                    c.Initialize();
                });
                if (Terminal.Instance.CurrentClient.ModuleList.Exists(c => c.TerminalStatus == ModuleStatus.NeedsSync))
                {
                    Terminal.Instance.CurrentClient.Notifier.SetUserMessage(true, "Algunas configuraciones pueden no estar sincronizadas con el servidor,\n"
                                                    + "por favor conectese con la central lo antes posible!");
                }
                else
                {
                    initialized = true;
                }
            }

            return initialized;

        }

        #endregion


    }
}
