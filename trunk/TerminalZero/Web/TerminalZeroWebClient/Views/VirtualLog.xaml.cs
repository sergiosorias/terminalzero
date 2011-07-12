using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Printing;
using TerminalZeroWebClient.ServiceHelperReference;
using ZeroGUI;

namespace TerminalZeroWebClient.Views
{
    public partial class VirtualLog : Page
    {

        ServiceHelperClient _client;
        
        public VirtualLog()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _client = new ServiceHelperClient();
            _client.GetLogsCompleted += client_GetLogsCompleted;
        }

        void client_GetLogsCompleted(object sender, GetLogsCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    var taskListView = new PagedCollectionView(e.Result);
                    if (taskListView.CanGroup)
                    {
                        if (cbGroupByIndend.IsChecked.HasValue && cbGroupByIndend.IsChecked.Value)
                        {
                            var group = new PropertyGroupDescription();
                            group.PropertyName = "IndentLevel";
                            taskListView.GroupDescriptions.Add(group);
                        }

                        if (cbGroupByMessage.IsChecked.HasValue && cbGroupByMessage.IsChecked.Value)
                        {
                            var group = new PropertyGroupDescription();
                            group.PropertyName = "Message";
                            taskListView.GroupDescriptions.Add(group);
                        }
                        
                    }
                    
                    logEntryEventArgsDataGrid.ItemsSource = taskListView;
                    if(!string.IsNullOrWhiteSpace(searchBox.txtSearchCriteria.Text))
                    {
                        SearchBox_Search(null, new SearchCriteriaEventArgs(searchBox.txtSearchCriteria.Text));
                    }

                    waitCursor.IsWaitEnable = false;
                });
        }
        
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            waitCursor.IsWaitEnable = true;
            _client.GetLogsAsync(DateTime.Now);
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            var taskListView = logEntryEventArgsDataGrid.ItemsSource as PagedCollectionView;

            if (taskListView!=null)
            {
                if (taskListView.CanFilter)
                {
                    taskListView.Filter = new Predicate<object>(i =>
                    {
                        var entry = i as VirtualLogEntry;
                        return (entry != null && entry.Message.ToUpper().Contains(e.Criteria.ToUpper()));
                    });

                    e.Matches = taskListView.ItemCount;
                }
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var pd = new PrintDocument();
            pd.PrintPage += (s, args) => 
            {
                args.PageVisual = logEntryEventArgsDataGrid;
            };

            pd.Print("Logs TZero");

        }

    }

    

}
