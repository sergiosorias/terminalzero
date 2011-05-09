using System;
using System.Diagnostics;
using System.Windows;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Pages.Controls;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for SupplierLazyLoadingList.xaml
    /// </summary>
    public partial class CustomerView : NavigationBasePage
    {
        public CustomerView()
        {
            ControlMode = ControlMode.ReadOnly;
            InitializeComponent();
            CommandBar.New += toolbar_New;
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            customerGrid.ApplyFilter(e.Criteria);
        }

        private void toolbar_New(object sender, RoutedEventArgs e)
        {
            var detail = new CustomerDetail();
            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.CustomerNew, ResizeMode.NoResize, MessageBoxButton.OKCancel);
            if (ret.HasValue && ret.Value)
            {
                try
                {
                    customerGrid.AddItem(detail.CurrentCustomer);
                }
                catch (Exception wx)
                {
                    MessageBox.Show(wx.ToString());
                    Trace.TraceError("Error updating Customer {0}", detail.CurrentCustomer);
                }
            }
        }

       
    }
}
