using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.ServiceModel;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using TerminalZeroClient.Business;
using System.IO;
using ZeroCommonClasses.GlobalObjects;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ITerminal
    {
        internal class Directories
        {
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

            public const string WorkingDirSubfix = ".WD";
        }

        public App()
            : base()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            AppDomainSetup set = new AppDomainSetup();
            set.PrivateBinPath = Directories.ModulesFolder;
            AppDomain.CreateDomain("Modules folder", null, set);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.EventLog.WriteEntry(Name, e.ToString());
        }

        private static App _CurrentApp = null;
        public static App Instance
        {
            get
            {
                if (_CurrentApp == null)
                {
                    _CurrentApp = (App)App.Current;
                    _CurrentApp.LoadParams();
                }

                return _CurrentApp;
            }
        }

        private void LoadParams()
        {
            CurrentClient = new ZeroClient();
            SetSession(new ZeroSession());
            _TerminalCode = TerminalZeroClient.Properties.Settings.Default.TerminalCode;
        }

        private ISyncService _ClientConn;
        private ISyncService ClientSyncServiceReference
        {
            get
            {
                if (_ClientConn == null)
                    _ClientConn = ZeroCommonClasses.Context.ContextBuilder.CreateSyncConnection();
                //int times = 3;
                //while (times > 0 && ((ICommunicationObject)_ClientConn).State == System.ServiceModel.CommunicationState.Faulted)
                //{
                //    _ClientConn = ZeroCommonClasses.Context.ContextBuilder.CreateConfigConnection(System.IO.Path.Combine(Environment.CurrentDirectory, "TerminalZeroClient.exe.config"));
                //    times--;
                //}

                return _ClientConn;
            }
        }

        internal ZeroClient CurrentClient { get; private set; }

        public string Name { get { return "Terminal Zero"; } }
        #region ITerminal Members
        private int _TerminalCode = -1;
        
        public int TerminalCode
        {
            get { return _TerminalCode; }
        }

        public string TerminalName
        {
            get { return Environment.MachineName; }
        }

        private ZeroSession _Session = null;
        public ZeroSession Session
        {
            get { return _Session; }
        }

        #endregion

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }
        
        internal void SetSession(ZeroSession zeroSession)
        {
            _Session = zeroSession;
            Session.AddNavigationParameter(new ZeroActionParameter<ISyncService>(false, ClientSyncServiceReference, false));
            Session.AddNavigationParameter(new ZeroActionParameter<IFileTransfer>(false, ZeroCommonClasses.Context.ContextBuilder.CreateFileTranferConnection(), false));    
        }
    }
}
