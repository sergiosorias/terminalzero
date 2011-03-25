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
    public partial class ProductsView : ZeroBasePage
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
                productList.AddProduct(detail.CurrentProduct);
            }
        }

        protected override void OnModeChanged()
        {
 	         base.OnModeChanged();
             productList.ControlMode = ControlMode;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            switch (ControlMode)
            {
                case ControlMode.New:
                    break;
                case ControlMode.Update:
                    break;
                case ControlMode.Delete:
                    break;
                case ControlMode.ReadOnly:
                    toolbar.NewBtnVisible = false;
                    break;
                default:
                    break;
            }
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = productList.ApplyFilter(e.Criteria);
        }

        
    }
}
