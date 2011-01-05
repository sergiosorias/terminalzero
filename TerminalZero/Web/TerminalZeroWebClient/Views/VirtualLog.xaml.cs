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
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Threading;

namespace TerminalZeroWebClient.Views
{
    public partial class VirtualLog : Page
    {
        const int k_timerTick = 50;
        ServiceHelperReference.ServiceHelperClient client;
        Timer timer;
        int timerTick = 0;
        public VirtualLog()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            remainingTime.Maximum = k_timerTick;
            timer = new Timer(new TimerCallback(Refresh), null, 1000, 500);
            client = new ServiceHelperReference.ServiceHelperClient();
            client.GetLogsCompleted += new EventHandler<ServiceHelperReference.GetLogsCompletedEventArgs>(client_GetLogsCompleted);
            
        }

        private void Refresh(object obj)
        {
            timerTick++;
            if (timerTick == k_timerTick)
            {
                timerTick = 0;
                client.GetLogsAsync(DateTime.Now);
            }
            Dispatcher.BeginInvoke(() =>
                {
                    remainingTime.Value = timerTick;
                });
            
        }

        void client_GetLogsCompleted(object sender, ServiceHelperReference.GetLogsCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    logEntryEventArgsDataGrid.ItemsSource = e.Result;
                });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            client.GetLogsAsync(DateTime.Now);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }

        
    }
}
