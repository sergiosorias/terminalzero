using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using TerminalZeroClient.Business;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var set = new AppDomainSetup { PrivateBinPath = ConfigurationContext.Directories.ModulesFolder };
            AppDomain.CreateDomain("Modules folder", null, set);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Terminal.Instance.CurrentClient = new WpfClient();
        }

        void App_LoadCompleted(object sender, NavigationEventArgs e)
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