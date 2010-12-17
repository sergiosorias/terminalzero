using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Forms;
using ZeroCommonClasses.Interfaces;

namespace TerminalZeroClient.Helpers
{
    internal class MainStatusNotifier : IProgressNotifier
    {
        ContentControl Process;
        System.Windows.Controls.ProgressBar Progress;
        ContentControl MandatyIcon;
        Dispatcher Dispatcher;

        public string LastMessage { get; set; }
        public StringBuilder Messages { get; private set; }

        public MainStatusNotifier(Dispatcher disp, ContentControl process, System.Windows.Controls.ProgressBar progress, ContentControl mandatyIcon)
        {
            this.Dispatcher = disp;
            Progress = progress;
            Process = process;
            MandatyIcon = mandatyIcon;

            Messages = new StringBuilder();
        }

        #region IProgressNotifier Members

        public void SetProcess(string newProgress)
        {
            this.Dispatcher.Invoke(new MethodInvoker(delegate() { Process.Content = newProgress; }), null);
            this.Messages.AppendLine(GetStamp() + newProgress);
        }

        private static string GetStamp()
        {
            return DateTime.Now.ToString("hh:mm:ss") + " - ";
        }

        public void SetProgress(int newProgress)
        {
            this.Dispatcher.Invoke(new MethodInvoker(delegate() { Progress.Value = newProgress; }), null);
        }

        public void SetUserMessage(bool isMandatory, string message)
        {
            this.Dispatcher.Invoke(new MethodInvoker(delegate() 
                {
                    if (isMandatory)
                    {
                        MandatyIcon.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                        MandatyIcon.Visibility = System.Windows.Visibility.Hidden;
                }), null);

            LastMessage = message;
            this.Messages.AppendLine(GetStamp()+LastMessage);
        }

        public void SendUserMessage(string message)
        {
            this.Dispatcher.Invoke(new MethodInvoker(delegate() { MessageBox.Show(message,"Informacion importante",MessageBoxButtons.OK,MessageBoxIcon.Information); }), null);
        }

        public event EventHandler ExecutionFinished;

        public void NotifyExecutionFinished(object sender)
        {
            if (ExecutionFinished != null)
                ExecutionFinished(sender, EventArgs.Empty);
        }

        #endregion
    }
}
