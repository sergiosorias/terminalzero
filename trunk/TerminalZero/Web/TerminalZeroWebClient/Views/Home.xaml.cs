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
        }

        

        protected void ClientGetTerminalsStatusCompleted(object sender, ServiceHelperReference.GetTerminalsStatusCompletedEventArgs e)
        {
            if (e.Result.IsValid)
            {
                terminalContent.Children.Clear();
                foreach (var item in e.Result.Result)
                {
                    var terminalStatus = new Controls.TerminalStatus
                        {
                            Margin = new Thickness(1, 1.5, 1, 1.5),
                            DataContext = item,
                            Width = 300
                        };
                    terminalContent.Children.Add(terminalStatus);
                }
                
            }
            waitCursorHome.Stop();
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            waitCursorHome.Start();
            _client.GetTerminalsStatusAsync();
        }
    }
}