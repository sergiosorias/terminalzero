using System;
using System.Diagnostics;
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
    public partial class App : Application
    {
        public App()
        {
            var set = new AppDomainSetup { PrivateBinPath = ContextInfo.Directories.ModulesFolder };
            AppDomain.CreateDomain("Modules folder", null, set);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ZeroCommonClasses.Terminal.Instance.CurrentClient = new TerminalClient();
        }

        void App_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

        public static string Name
        {
            get { return TerminalZeroClient.Properties.Resources.AppName; }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.TraceError(Name, e.ToString());
            EventLog.WriteEntry(Name, e.ToString());
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }

        
        
    }
}