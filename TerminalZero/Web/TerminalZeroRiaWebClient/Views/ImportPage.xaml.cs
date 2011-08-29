using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using TerminalZeroRiaWebClient.ViewModels;
using TerminalZeroRiaWebClient.Web.Services;
using ZeroCommonClasses.Entities;
using ZeroGUI;

namespace TerminalZeroRiaWebClient.Views
{
    public partial class ImportPage : Page
    {
        public ImportPage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
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
                ((ImportPageViewModel) LayoutRoot.DataContext).UploadPack(ofdlg.File.Name, ofdlg.File.OpenRead());
            }
            
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

        private void searchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            PagedCollectionView taskListView = null;//packDataGrid.ItemsSource as PagedCollectionView;

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
            Pack pack = ((Pack)packDataGrid.SelectedItem);
            if (pack.Data != null)
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = "Zip File (.zip)|*.zip";
                if (sfd.ShowDialog().GetValueOrDefault())
                {
                    using (Stream fs = sfd.OpenFile())
                    {
                        fs.Write(pack.Data, 0, pack.Data.Length);
                    }
                }
            }
            
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Button ele = sender as Button;
            DataGridRow row = SLFramework.ControlsExtentions.FindAncestor<DataGridRow>(ele);
            if (row != null)
            {
                row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Pack pack = ((Pack) packDataGrid.SelectedItem);
            if (MessageBox.Show("¿Esta seguro de reprocesar el paquete?", string.Format("Re-proceso pack {0}", pack.Code), MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                
            }
        }
    }
}
