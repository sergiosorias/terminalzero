using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ZeroCommonClasses.Interfaces;

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
            appName.Content = Properties.Settings.Default.ApplicationName;
            App.Instance.Session.Notifier = this;
            BackgroundWorker work = new BackgroundWorker();
            work.DoWork += work_DoWork;
            work.RunWorkerCompleted += work_RunWorkerCompleted;
            work.RunWorkerAsync();
            
        }
        
        void work_DoWork(object sender, DoWorkEventArgs e)
        {
            App.Instance.CurrentClient.InitializeAppAsync();
        }

        void work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Action action;
            if (App.Instance.CurrentClient.IsAllOK && !ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose)
            {
                action = delegate() { btnState_Click(null, null); };
            }
            else
            {
                action = delegate() { btnState.Visibility = System.Windows.Visibility.Visible; };
            }

            this.Dispatcher.Invoke(action, null);
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
