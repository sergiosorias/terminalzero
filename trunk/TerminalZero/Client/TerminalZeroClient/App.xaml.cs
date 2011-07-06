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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Terminal.Instance.CurrentClient = new WpfClient();
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
            Terminal.Instance.CurrentClient.Dispose();
            Terminal.Instance.CurrentClient = null;
        }

        
        
    }
}