using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TerminalZeroClient.Properties;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.Interfaces;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for SplashView.xaml
    /// </summary>
    public partial class SplashView : Window, IProgressNotifier
    {
        StringBuilder fullLog = new StringBuilder();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            appName.Content = Settings.Default.ApplicationName;
            Terminal.Instance.CurrentClient.Notifier = this;
            var work = new BackgroundWorker();
            work.DoWork += (o, args) =>
                               {
                                   if (Terminal.Instance.CurrentClient.Initialize())
                                   {
                                       Action action;
                                       if (!ConfigurationContext.LogLevel.TraceVerbose)
                                       {
                                           action = delegate() { btnState_Click(null, null); };
                                       }
                                       else
                                       {
                                           action = delegate() { btnState.Visibility = Visibility.Visible; };
                                       }

                                       Dispatcher.Invoke(action, null);
                                   }
                                   else
                                   {
                                       btnClose.Visibility = Visibility.Visible;
                                   }
                               };
            work.RunWorkerAsync();
        }
        
        
        #region IProgressNotifier Members

        public void SetProcess(string newProgress)
        {
            fullLog.AppendLine(newProgress);
            Dispatcher.Invoke(new MethodInvoker(delegate { statusMsg.Content = newProgress; }), null);
        }

        public void SetProgress(int newProgress)
        {
            Dispatcher.Invoke(new MethodInvoker(delegate { statusBar.Value = newProgress; }), null);
        }

        public void SetUserMessage(bool isMandatory, string message)
        {
            fullLog.AppendLine(message);
            Dispatcher.Invoke(new MethodInvoker(delegate
                                                    {
                tbxLog.Text += "\n" + message;
                tbxLog.ScrollToEnd();
            }), null);
        }

        public void SendNotification(string message)
        {
            fullLog.AppendLine(message);
            Dispatcher.Invoke(new MethodInvoker(delegate { MessageBox.Show(message, Properties.Resources.ImportantInformation, MessageBoxButtons.OK, MessageBoxIcon.Information); }), null);
        }

        public void Log(TraceLevel level, string message)
        {

        }

        #endregion

        private void btnState_Click(object sender, RoutedEventArgs e)
        {
            Terminal.Instance.CurrentClient.Load();
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                switch (e.SystemKey)
                {
                    case Key.L:
                        Width = 400;
                        Height = 500;
                        tbxLog.Text = fullLog.ToString();
                        fullLog.Length = 0;
                        break;
                    case Key.R:
                        tbxLog.Text = string.Empty;
                        break;
                    default:
                        break;
                }
            }

        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        

        
    }
}
