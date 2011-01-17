using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TerminalZeroWebClient
{
    public partial class Home : Page
    {
        ServiceHelperReference.ServiceHelperClient client;
        public Home()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            client = new ServiceHelperReference.ServiceHelperClient();
            
            client.GetTerminalsStatusCompleted += new EventHandler<ServiceHelperReference.GetTerminalsStatusCompletedEventArgs>(client_GetTerminalsStatusCompleted);
            client.GetTerminalsStatusAsync();
            waitCursorHome.Start();
        }

        void client_GetTerminalsStatusCompleted(object sender, ServiceHelperReference.GetTerminalsStatusCompletedEventArgs e)
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
    }
}