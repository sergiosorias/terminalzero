using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ZeroBusiness;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroConfiguration.Pages;
using ZeroConfiguration.Pages.Controls;
using ZeroConfiguration.Presentantion;
using ZeroConfiguration.Properties;
using ZeroGUI;
using ZeroPrinters;
using Terminal = ZeroCommonClasses.Terminal;

namespace ZeroConfiguration
{
    public class ZeroConfigurationModule : ZeroModule, ITerminalManager
    {
        private const string K_administrator = "Administrator";
        private const string K_password = "admin";

        private Synchronizer _sync;
        private bool _isTerminalZero;
        public ZeroConfigurationModule()
            : base(2, Resources.ConfigurationModuleDescription)
        {
            Terminal.Instance.Manager = this;
        }

        public override void Initialize()
        {
            StartSyncronizer();
            ValidateAdminUser();
            Terminal.Instance.Client.Loaded += (o, e) => { OpenLogInDialog(); };
        }

        private List<PrinterInfo> LoadPrintersConfig(ConfigurationModelManager manager)
        {
            var infos = new List<PrinterInfo>();
            foreach (Printer printer in manager.Printers.ToList())
            {
                var aprinter = new PrinterInfo();
                aprinter.Name = printer.Name;
                aprinter.Type = printer.Type.HasValue ? printer.Type.Value : 1;
                aprinter.Parameters = new Dictionary<string, string>();
                foreach (var VARIABLE in printer.PrinterParameters)
                {
                    aprinter.Parameters.Add(VARIABLE.Name, VARIABLE.Value);
                }
                infos.Add(aprinter);
            }
            return infos;
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
#if DEBUG
            ActionParameterBase userpParam = new ActionParameter<User>(false, User.GetUser(K_administrator, true), false);
            Terminal.Instance.Session[userpParam.Name] = userpParam;
#else
            var view = new UserLogIn();
            Terminal.Instance.Client.ShowDialog(view,null, dialogResult =>
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
                        Terminal.Instance.Client.ShowDialog(Resources.MsgIncorrectUserPassTryAgain,Resources.Fail, (res) => { OpenLogInDialog(); }, MessageBoxButtonEnum.OK);
                    }
                }
                else
                {
                    Terminal.Instance.Client.ShowDialog(Resources.MsgLogInPlease + "\nEl sistema se cerrara.", Resources.Fail, (res) =>
                    {
                        if (Terminal.Instance.Session.Actions[Actions.AppExit] != null)
                        {
                            Terminal.Instance.Session.Actions[Actions.AppExit].TryExecute();
                        }                                                                                                         
                    }, MessageBoxButtonEnum.OK);
}
            });
#endif
        }

        private void StartSyncronizer()
        {
            _sync = new Synchronizer();
            double milsec;
            using (var conf = new ConfigurationModelManager())
            {
                milsec = _sync.LoadConfiguration(conf, Terminal.Instance.Session);
            }

            Terminal.Instance.Client.Notifier.SetUserMessage(false, string.Format(Resources.SyncEveryFormat, (milsec / 1000) / 60));
            _sync.SyncStarting += SyncSyncStarting;
            _sync.SyncFinished += _sync_SyncFinished;

            Terminal.Instance.Session.Actions[Actions.ExecSync].TryExecute();
        }

        private void _sync_SyncFinished(object sender, EventArgs e)
        {
            using (var conf = new ConfigurationModelManager())
            {
                _isTerminalZero = ConfigurationModelManager.IsTerminalZero(conf, Terminal.Instance.Code);
            }
            OnConfigurationRequired();
        }

        private void SyncSyncStarting(object sender, Synchronizer.SyncStartingEventArgs e)
        {
            e.Notifier = Terminal.Instance.Client.Notifier;
            e.SyncService = (ISyncService)Terminal.Instance.Session[typeof(ISyncService)].Value;
            e.FileTransferService = (IFileTransfer)Terminal.Instance.Session[typeof(IFileTransfer)].Value;
            e.Modules = Terminal.Instance.Client.ModuleList;
        }

        [ZeroAction(Actions.ExecSync, null, true, false, false)]
        [ZeroActionParameter(typeof(ISyncService), true)]
        private void StartSync(object parameter)
        {
            _sync.Start();
        }

        [ZeroAction(Actions.AppHome, null, false, true, false)]
        private void OpenHomePage(object obj)
        {
            Terminal.Instance.Client.ShowView(new HomePage());
        }

        [ZeroAction(Actions.OpenUserListView, Rules.IsValidUser, true)]
        private void OpenUsers(object parameter)
        {
            Terminal.Instance.Client.ShowView(new Users());
        }

        [ZeroAction(Actions.OpenUserPasswordChangeMessage, null, true)]
        [ZeroActionParameter(typeof(User), true)]
        private void OpenChangePassword(object parameter)
        {
            var pswChange = new UserChangePassword();
            pswChange.DataContext = Terminal.Instance.Session[typeof(User)].Value;
            ZeroMessageBox.Show(pswChange, Resources.ChangePassword, ResizeMode.NoResize, MessageBoxButton.OKCancel);
        }

        [ZeroAction(Actions.OpenPropertiesView, null, true)]
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

            Terminal.Instance.Client.ShowView(view);
        }

        [ZeroRule(Rules.IsValidUser)]
        private bool CanOpenConfiguration(object param)
        {
            return true;
        }

        [ZeroRule(Rules.IsTerminalZero)]
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
            return ConfigurationModelManager.GetTerminalModuleStatus(new ConfigurationModelManager(), Terminal.Instance.Code, module);
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
                ZeroBusiness.Entities.Configuration.Terminal T = conf.Terminals.FirstOrDefault(t => t.Code == Terminal.Instance.Code);
                if (T == null)
                {
                    ZeroBusiness.Entities.Configuration.Terminal.AddNewTerminal(conf, Terminal.Instance.Code, Terminal.Instance.TerminalName);
                }
                else
                {
                    _isTerminalZero = T.IsTerminalZero;
                    if (Terminal.Instance.Code == 0 && !T.IsTerminalZero)
                    {
                        T.IsTerminalZero = true;
                        conf.SaveChanges();
                    }
                }
                ConfigurationModelManager.CreateTerminalProperties(conf, Terminal.Instance.Code);

                Terminal.Instance.Client.ModuleList.ForEach(c =>
                {
                    c.TerminalStatus = GetModuleStatus(c);
                    c.Initialize();
                });
                if (Terminal.Instance.Client.ModuleList.Exists(c => c.TerminalStatus == ModuleStatus.NeedsSync))
                {
                    Terminal.Instance.Client.Notifier.SetUserMessage(true, "Algunas configuraciones pueden no estar sincronizadas con el servidor,\n"
                                                    + "por favor conectese con la central lo antes posible!");
                }
                else
                {
                    initialized = true;
                }

                if (!TerminalPrinters.Instance.Load(LoadPrintersConfig(conf)))
                {
                    Terminal.Instance.Client.Notifier.SendNotification(TerminalPrinters.Instance.Error);
                }
            }

            return initialized;

        }

        #endregion


    }
}
