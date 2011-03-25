using System;
using System.Diagnostics;
using System.Windows;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Pages.Controls;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for SupplierList.xaml
    /// </summary>
    public partial class CustomerView : ZeroBasePage
    {
        public CustomerView()
        {
            ControlMode = ControlMode.ReadOnly;
            InitializeComponent();
        }

        private void btnNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            var detail = new CustomerDetail(customerGrid.DataProvider);
            bool? ret = ZeroMessageBox.Show(detail,Properties.Resources.CustomerNew,ResizeMode.NoResize, MessageBoxButton.OKCancel);
            if (ret.HasValue && ret.Value)
            {
                try
                {
                    customerGrid.RefreshList(detail.CurrentCustomer);
                }
                catch (Exception wx)
                {
                    MessageBox.Show(wx.ToString());
                    Trace.TraceError("Error updating Customer {0}", detail.CurrentCustomer);
                }
            }
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            customerGrid.ApplyFilter(e.Criteria);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        
    }
}
