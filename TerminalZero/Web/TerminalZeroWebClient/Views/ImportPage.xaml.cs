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
using System.IO;
using System.Windows.Data;

namespace TerminalZeroWebClient.Views
{
    public partial class ImportPage : Page
    {
        ServiceHelperReference.ServiceHelperClient client;

        public ImportPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            client = new ServiceHelperReference.ServiceHelperClient();
            client.GetPackCompleted += new EventHandler<ServiceHelperReference.GetPackCompletedEventArgs>(client_GetPackCompleted);
            startDate.SelectedDate = DateTime.Now.Date;
            endDate.SelectedDate = DateTime.Now.AddDays(1).Date;
        }

        private void client_GetPackCompleted(object sender, ServiceHelperReference.GetPackCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                PagedCollectionView taskListView = new PagedCollectionView(e.Result);
                if (taskListView.CanGroup)
                {
                    

                }

                packDataGrid.ItemsSource = taskListView;
                if (!string.IsNullOrWhiteSpace(searchBox.txtSearchCriteria.Text))
                {
                    searchBox_Search(null, new ZeroGUI.SearchCriteriaEventArgs(searchBox.txtSearchCriteria.Text));
                }
            });
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdlg = new OpenFileDialog();
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
            UriBuilder ub = new UriBuilder(new Uri(Application.Current.Host.Source, "../filereceiver.ashx"));
            ub.Query = string.Format("filename={0}", fileName.Name);

            WebClient c = new WebClient();
            c.OpenWriteCompleted += (sender, e) =>
            {
                PushData(fileName.Length, data, e.Result);
                e.Result.Close();
                data.Close();
            };
            c.OpenWriteAsync(ub.Uri);

        }

        private void PushData(long totalLength, Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
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

        private void searchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            PagedCollectionView taskListView = packDataGrid.ItemsSource as PagedCollectionView;

            if (taskListView != null)
            {
                if (taskListView.CanFilter)
                {
                    taskListView.Filter = new Predicate<object>(i =>
                    {
                        ServiceHelperReference.Pack entry = i as ServiceHelperReference.Pack;
                        return (entry != null && entry.Name.ToUpper().Contains(e.Criteria.ToUpper())
                            || (entry.Result != null && entry.Result.ToUpper().Contains(e.Criteria.ToUpper())));
                    });

                    e.Matches = taskListView.ItemCount;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Zip File (.zip)|*.zip";
            if (sfd.ShowDialog().GetValueOrDefault())
            {
                using (Stream fs = sfd.OpenFile())
                {
                    byte[] file = (byte[])((Button)sender).DataContext;
                    fs.Write(file, 0, file.Length);
                }
            }
        }

    }
}
