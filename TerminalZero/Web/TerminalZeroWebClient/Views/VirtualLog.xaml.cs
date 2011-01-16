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
using System.Windows.Data;
using System.Windows.Printing;

namespace TerminalZeroWebClient.Views
{
    public partial class VirtualLog : Page
    {

        ServiceHelperReference.ServiceHelperClient client;
        
        public VirtualLog()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            client = new ServiceHelperReference.ServiceHelperClient();
            client.GetLogsCompleted += new EventHandler<ServiceHelperReference.GetLogsCompletedEventArgs>(client_GetLogsCompleted);
        }

        void client_GetLogsCompleted(object sender, ServiceHelperReference.GetLogsCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    PagedCollectionView taskListView = new PagedCollectionView(e.Result);
                    if (taskListView.CanGroup)
                    {
                        if (cbGroupByIndend.IsChecked.HasValue && cbGroupByIndend.IsChecked.Value)
                        {
                            PropertyGroupDescription group = new PropertyGroupDescription();
                            group.PropertyName = "IndentLevel";
                            taskListView.GroupDescriptions.Add(group);
                        }

                        if (cbGroupByMessage.IsChecked.HasValue && cbGroupByMessage.IsChecked.Value)
                        {
                            PropertyGroupDescription group = new PropertyGroupDescription();
                            group.PropertyName = "Message";
                            taskListView.GroupDescriptions.Add(group);
                        }
                        
                    }
                    
                    logEntryEventArgsDataGrid.ItemsSource = taskListView;
                    if(!string.IsNullOrWhiteSpace(searchBox.txtSearchCriteria.Text))
                    {
                        SearchBox_Search(null, new ZeroGUI.SearchCriteriaEventArgs(searchBox.txtSearchCriteria.Text));
                    }
                });
        }
        
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            client.GetLogsAsync(DateTime.Now);
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            PagedCollectionView taskListView = logEntryEventArgsDataGrid.ItemsSource as PagedCollectionView;

            if (taskListView!=null)
            {
                if (taskListView.CanFilter)
                {
                    taskListView.Filter = new Predicate<object>(i =>
                    {
                        ServiceHelperReference.VirtualLogEntry entry = i as ServiceHelperReference.VirtualLogEntry;
                        return (entry != null && entry.Message.ToUpper().Contains(e.Criteria.ToUpper()));
                    });

                    e.Matches = taskListView.ItemCount;
                }
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, args) => 
            {
                args.PageVisual = logEntryEventArgsDataGrid;
            };

            pd.Print("Logs TZero");

        }

    }

    public class DoubleFormatter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            // Retrieve the format string and use it to format the value.
            try
            {
                double d = double.Parse(value.ToString());
                return (int)d;
            }
            catch (Exception)
            {

            }
            return value;

            // If the format string is null or empty, simply call ToString()
            // on the value.

        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
