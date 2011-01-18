using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TerminalZeroWebClient.Views
{
    public partial class Home : Page
    {
        ServiceHelperReference.ServiceHelperClient _client;
        public Home()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _client = new ServiceHelperReference.ServiceHelperClient();
            
            _client.GetTerminalsStatusCompleted += ClientGetTerminalsStatusCompleted;
            _client.GetTerminalsStatusAsync();
            waitCursorHome.Start();
        }

        protected void ClientGetTerminalsStatusCompleted(object sender, ServiceHelperReference.GetTerminalsStatusCompletedEventArgs e)
        {
            if (e.Result.IsValid)
            {
                terminalContent.Children.Clear();
                foreach (var item in e.Result.Result)
                {
                    Controls.TerminalStatus T = new Controls.TerminalStatus();
                    T.Margin = new Thickness(1,1.5,1,1.5);
                    T.DataContext = item;
                    T.Width = 300;
                    terminalContent.Children.Add(T);
                }
                
            }
            waitCursorHome.Stop();
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            _client.GetTerminalsStatusAsync();
        }
    }
}