using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZeroCommonClasses.Interfaces;
using System.Windows.Forms;
using System.ComponentModel;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for SplashView.xaml
    /// </summary>
    public partial class SplashView : Window, IProgressNotifier
    {
        StringBuilder fullLog = new StringBuilder();
        public SplashView()
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            appName.Content = Properties.Settings.Default.ApplicationName;
            App.Instance.CurrentClient.Session.Notifier = this;
            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += new DoWorkEventHandler(work_DoWork);
            work.RunWorkerCompleted += new RunWorkerCompletedEventHandler(work_RunWorkerCompleted);
            work.RunWorkerAsync();
        }
        
        void work_DoWork(object sender, DoWorkEventArgs e)
        {
            App.Instance.CurrentClient.InitializeAppAsync();
        }

        void work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (App.Instance.CurrentClient.IsAllOK && !App.Instance.IsOnDebugMode)
            {
                this.Dispatcher.Invoke(new MethodInvoker(delegate() { btnState_Click(null, null); }), null);
            }
            else
            {
                this.Dispatcher.Invoke(new MethodInvoker(delegate() { btnState.Visibility = System.Windows.Visibility.Visible; }), null);
            }
        }

        #region IProgressNotifier Members

        public void SetProcess(string newProgress)
        {
            fullLog.AppendLine(newProgress);
            this.Dispatcher.Invoke(new MethodInvoker(delegate() { statusMsg.Content = newProgress; }), null);
        }

        public void SetProgress(int newProgress)
        {
            this.Dispatcher.Invoke(new MethodInvoker(delegate() { statusBar.Value = newProgress; }), null);
        }

        public void SetUserMessage(bool isMandatory, string message)
        {
            fullLog.AppendLine(message);
            this.Dispatcher.Invoke(new MethodInvoker(delegate()
            {
                tbxLog.Text += "\n" + message;
                tbxLog.ScrollToEnd();
            }), null);
        }

        public void SendNotification(string message)
        {
            fullLog.AppendLine(message);
            this.Dispatcher.Invoke(new MethodInvoker(delegate() { System.Windows.Forms.MessageBox.Show(message, "Informacion importante", MessageBoxButtons.OK, MessageBoxIcon.Information); }), null);
        }

        public void Log(System.Diagnostics.TraceLevel level, string message)
        {

        }

        #endregion

        private void btnState_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();
            this.Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                switch (e.SystemKey)
                {
                    case Key.L:
                        this.Width = 400;
                        this.Height = 500;
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
            this.DragMove();
        }

        

        
    }
}
