using System;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for SupplierList.xaml
    /// </summary>
    public partial class CustomerView : UserControl, IZeroPage
    {
        public CustomerView()
        {
            Mode = Mode.ReadOnly;
            InitializeComponent();
        }

        private void btnNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            Controls.CustomerDetail detail = new Controls.CustomerDetail(customerGrid.DataProvider);
            bool? ret = ZeroMessageBox.Show(detail);
            if (ret.HasValue && ret.Value)
            {
                try
                {
                    customerGrid.RefreshList(detail.CurrentCustomer);
                }
                catch (Exception wx)
                {
                    MessageBox.Show(wx.ToString());
                }
            }
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            customerGrid.ApplyFilter(e.Criteria);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        #region IZeroPage Members

        public Mode Mode { get; set; }

        public bool CanAccept()
        {
            return true;
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion
                
        
    }
}
