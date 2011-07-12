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
                terminalList.ItemsSource = e.Result.Result;
            }
            waitCursorHome.IsWaitEnable = false;
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            waitCursorHome.IsWaitEnable = true;
            _client.GetTerminalsStatusAsync();
        }
    }
}