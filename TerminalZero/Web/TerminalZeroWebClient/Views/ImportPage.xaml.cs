using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using TerminalZeroWebClient.FileTranferReference;
using TerminalZeroWebClient.ServiceHelperReference;
using ZeroGUI;

namespace TerminalZeroWebClient.Views
{
    public partial class ImportPage : Page
    {
        ServiceHelperClient client;
        FileTransferClient _uploadClient = new FileTransferClient();
        public ImportPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            client = new ServiceHelperClient();
            client.GetPackCompleted += client_GetPackCompleted;
            _uploadClient.UploadFileSilverlightCompleted += _uploadClient_UploadFileSilverlightCompleted;
            startDate.SelectedDate = DateTime.Now.Date;
            endDate.SelectedDate = DateTime.Now.AddDays(1).Date;
        }

        private void client_GetPackCompleted(object sender, GetPackCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var taskListView = new PagedCollectionView(e.Result);
                if (taskListView.CanGroup)
                {
                    

                }

                packDataGrid.ItemsSource = taskListView;
                if (!string.IsNullOrWhiteSpace(searchBox.txtSearchCriteria.Text))
                {
                    searchBox_Search(null, new SearchCriteriaEventArgs(searchBox.txtSearchCriteria.Text));
                }
            });
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            var ofdlg = new OpenFileDialog();
            fileProgress.Value = 0;
            ofdlg.Filter = "Zip File (.zip)|*.zip";
            ofdlg.Multiselect = false;
            bool? res = ofdlg.ShowDialog();
            if (res.HasValue && res.Value)
            {
                UploadFile(ofdlg.File, ofdlg.File.OpenRead());
            }
            
        }

        private void UploadFile(FileInfo fileName, Stream data)
        {
            //UriBuilder ub = new UriBuilder(new Uri(Application.Current.Host.Source, "../filereceiver.ashx"));
            //ub.Query = string.Format("filename={0}", fileName.Name);

            //WebClient c = new WebClient();
            //c.OpenWriteCompleted += (sender, e) =>
            //{
            //    PushData(fileName.Length, data, e.Result);
            //    e.Result.Close();
            //    data.Close();
            //};
            //c.OpenWriteAsync(ub.Uri);
            //OperationContext.Current.OutgoingMessageHeaders
            BinaryReader br = new BinaryReader(data);
            _uploadClient.UploadFileSilverlightAsync(fileName.Name, br.ReadBytes((int)fileName.Length));
        }
        
        private void _uploadClient_UploadFileSilverlightCompleted(object sender, UploadFileSilverlightCompletedEventArgs e)
        {
            fileProgress.Value = 100;
        }

        private void PushData(long totalLength, Stream input, Stream output)
        {
            var buffer = new byte[4096];
            int bytesRead;
            long total = 0;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                total += bytesRead;
                fileProgress.Value = (total * 100 / totalLength);
                output.Write(buffer, 0, bytesRead);
                output.Flush();
            }
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            client.GetPackAsync(startDate.SelectedDate.GetValueOrDefault(), endDate.SelectedDate.GetValueOrDefault());
        }

        private void searchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            var taskListView = packDataGrid.ItemsSource as PagedCollectionView;

            if (taskListView != null)
            {
                if (taskListView.CanFilter)
                {
                    taskListView.Filter = new Predicate<object>(i =>
                    {
                        var entry = i as Pack;
                        return (entry != null && entry.Name.ToUpper().Contains(e.Criteria.ToUpper())
                            || (entry.Result != null && entry.Result.ToUpper().Contains(e.Criteria.ToUpper())));
                    });

                    e.Matches = taskListView.ItemCount;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Zip File (.zip)|*.zip";
            if (sfd.ShowDialog().GetValueOrDefault())
            {
                using (Stream fs = sfd.OpenFile())
                {
                    var file = (byte[])((Button)sender).DataContext;
                    fs.Write(file, 0, file.Length);
                }
            }
        }

        private void packDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // the original source is what was clicked.  For example  
                // a button. 
                DependencyObject dep = (DependencyObject)e.OriginalSource;

                // iteratively traverse the visual tree upwards looking for 
                // the clicked row. 
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                // if we found the clicked row 
                if (dep != null && dep is DataGridRow)
                {
                    // get the row 
                    DataGridRow row = (DataGridRow)dep;

                    // change the details visibility 
                    if (row.DetailsVisibility == Visibility.Collapsed)
                    {
                        row.DetailsVisibility = Visibility.Visible;
                    }
                    else
                    {
                        row.DetailsVisibility = Visibility.Collapsed;
                    }
                }
            }
            catch (System.Exception)
            {
            } 

        }

    }
}
