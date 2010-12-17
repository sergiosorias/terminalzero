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

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ITerminal
    {
        public static string K_ModulesFolder = "Modules";
        public App() 
            :base()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            AppDomainSetup set = new AppDomainSetup();
            set.PrivateBinPath = System.IO.Path.Combine(Environment.CurrentDirectory,"Modules");
            AppDomain.CreateDomain("Modules folder",null,set);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
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
            CurrentClient = new ZeroClientManager();
            _TerminalCode = TerminalZeroClient.Properties.Settings.Default.TerminalCode;
            IsOnDebugMode = TerminalZeroClient.Properties.Settings.Default.Debug;
        }

        private ISyncService _ClientConn;
        internal ISyncService ClientSyncServiceReference
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
        internal ZeroClientManager CurrentClient { get; private set; }

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

        #endregion


        public bool IsOnDebugMode { get; set; }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            
        }
    }
}
