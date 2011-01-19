using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TerminalZeroClient.Business;
using TerminalZeroClient.Properties;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ITerminal
    {
        private static App _currentApp;

        private ISyncService _clientConn;
        private ITerminalManager _manager;
        private ZeroSession _session;
        private int _terminalCode = -1;

        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            var set = new AppDomainSetup {PrivateBinPath = Directories.ModulesFolder};
            AppDomain.CreateDomain("Modules folder", null, set);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public static App Instance
        {
            get
            {
                if (_currentApp == null)
                {
                    _currentApp = (App) Current;
                    _currentApp.LoadParams();
                }

                return _currentApp;
            }
        }

        private ISyncService ClientSyncServiceReference
        {
            get
            {
                return _clientConn ?? (_clientConn = ContextBuilder.CreateSyncConnection());
                //int times = 3;
                //while (times > 0 && ((ICommunicationObject)_ClientConn).State == System.ServiceModel.CommunicationState.Faulted)
                //{
                //    _ClientConn = ZeroCommonClasses.Context.ContextBuilder.CreateConfigConnection(System.IO.Path.Combine(Environment.CurrentDirectory, "TerminalZeroClient.exe.config"));
                //    times--;
                //}
            }
        }

        internal ZeroClient CurrentClient { get; private set; }

        public string Name
        {
            get { return TerminalZeroClient.Properties.Resources.AppName; }
        }

        #region ITerminal Members

        public int TerminalCode
        {
            get { return _terminalCode; }
        }

        public string TerminalName
        {
            get { return Environment.MachineName; }
        }

        public ZeroSession Session
        {
            get { return _session; }
        }

        public ITerminalManager Manager
        {
            get { return _manager; }
        }

        #endregion

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry(Name, e.ToString());
        }

        private void LoadParams()
        {
            CurrentClient = new ZeroClient();
            SetSession(new ZeroSession());
            _terminalCode = Settings.Default.TerminalCode;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }

        internal void SetSession(ZeroSession zeroSession)
        {
            _session = zeroSession;
            Session.AddNavigationParameter(new ZeroActionParameter<ISyncService>(false, ClientSyncServiceReference,
                                                                                 false));
            Session.AddNavigationParameter(new ZeroActionParameter<IFileTransfer>(false,
                                                                                  ContextBuilder.
                                                                                      CreateFileTranferConnection(),
                                                                                  false));
        }

        internal void SetManager(ITerminalManager zeroManager)
        {
            _manager = zeroManager;
        }

        #region Nested type: Directories

        internal class Directories
        {
            public const string WorkingDirSubfix = ".WD";

            static Directories()
            {
                try
                {
                    if (!Directory.Exists(ModulesFolder))
                        Directory.CreateDirectory(ModulesFolder);
                    if (!Directory.Exists(UpgradeFolder))
                        Directory.CreateDirectory(UpgradeFolder);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public static string ModulesFolder
            {
                get { return Path.Combine(Environment.CurrentDirectory, "Modules"); }
            }

            public static string UpgradeFolder
            {
                get { return Path.Combine(Environment.CurrentDirectory, "Upgrade"); }
            }
        }

        #endregion
    }
}