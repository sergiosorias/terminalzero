using System;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Pages.Controls;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class ProductsView : NavigationBasePage
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var detail = new ProductDetail(productList.DataProvider);

            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.NewProduct);
            if (ret.HasValue && ret.Value && detail.ControlMode == ControlMode.New)
            {
                productList.AddItem(detail.CurrentProduct);
            }
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
 	         base.OnControlModeChanged(newMode);
             toolbar.NewBtnVisible = !(ControlMode == ControlMode.ReadOnly);
             productList.ControlMode = ControlMode;
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = productList.ApplyFilter(e.Criteria);
        }
        
    }
}
