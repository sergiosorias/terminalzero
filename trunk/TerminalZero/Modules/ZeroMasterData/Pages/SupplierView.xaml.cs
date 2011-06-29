using System;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;


namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for SupplierLazyLoadingList.xaml
    /// </summary>
    public partial class SupplierView : NavigationBasePage
    {
        public SupplierView()
        {
            InitializeComponent();
            CommandBar.New += btnNewSupplier_Click;
        }

        private void btnNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            var detail = new Controls.SupplierDetail();
            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.NewSupplier);
            if (ret.HasValue && ret.Value)
            {
                try
                {
                    supplierGrid.AddItem(detail.CurrentSupplier);
                }
                catch (Exception wx)
                {
                    ZeroMessageBox.Show(wx.ToString(),"Error");
                }
            }
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            supplierGrid.ApplyFilter(e.Criteria);
        }

       
    }
}
