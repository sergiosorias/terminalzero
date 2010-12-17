using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                    global::System.Windows.Forms.MessageBox.Show(wx.ToString());
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

        private Mode _Mode = Mode.ReadOnly;
        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }

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
