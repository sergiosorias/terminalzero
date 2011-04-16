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

            waitCursor.Stop();
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
                BinaryReader br = new BinaryReader(ofdlg.File.OpenRead());
                UploadFile(ofdlg.File.Name, br.ReadBytes((int)ofdlg.File.Length));
            }
            
        }

        private void UploadFile(string fileName, byte[] data)
        {
            waitCursor.Start();
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
            _uploadClient.UploadFileSilverlightAsync(fileName, data);
        }
        
        private void _uploadClient_UploadFileSilverlightCompleted(object sender, UploadFileSilverlightCompletedEventArgs e)
        {
            waitCursor.Stop();
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
            waitCursor.Start();
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
                    Pack pack = ((Pack)packDataGrid.SelectedItem);
                    fs.Write(pack.Data, 0, pack.Data.Length);
                }
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Button ele = sender as Button;
            DataGridRow row = Classes.Extensions.FindAncestor<DataGridRow>(ele);
            if (row != null)
            {
                row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Pack pack = ((Pack) packDataGrid.SelectedItem);
            if(MessageBox.Show("¿Esta seguro de reprocesar el paquete?",string.Format("Re-proceso pack {0}",pack.Code),MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                UploadFile(pack.Name, pack.Data);
        }

        private void TextBox_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        
    }
}
