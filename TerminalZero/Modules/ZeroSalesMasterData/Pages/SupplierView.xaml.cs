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
            Controls.SupplierDetail detail = new Controls.SupplierDetail(supplierGrid.DataProvider);
            bool? ret = ZeroMessageBox.Show(detail);
            if (ret.HasValue && ret.Value)
            {
                try
                {
                    supplierGrid.RefreshList(detail.CurrentSupplier);
                }
                catch (Exception wx)
                {
                    global::System.Windows.Forms.MessageBox.Show(wx.ToString());
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
