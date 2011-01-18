using System;
using System.Windows;
using System.Windows.Controls;
using ZeroGUI;


namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for SupplierList.xaml
    /// </summary>
    public partial class SupplierView : UserControl
    {
        public SupplierView()
        {
            InitializeComponent();
        }

        private void btnNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            var detail = new Controls.SupplierDetail(supplierGrid.DataProvider);
            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.NewSupplier);
            if (ret.HasValue && ret.Value)
            {
                try
                {
                    supplierGrid.RefreshList(detail.CurrentSupplier);
                }
                catch (Exception wx)
                {
                    MessageBox.Show(wx.ToString());
                }
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            supplierGrid.ApplyFilter(e.Criteria);
        }
    }
}
