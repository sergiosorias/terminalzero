using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class ProductsView : UserControl, IZeroPage
    {
        public ProductsView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var detail = new Controls.ProductDetail(productList.DataProvider);

            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.NewProduct);
            if (ret.HasValue && ret.Value)
            {
                
            }
        }

        #region IZeroPage Members

        private Mode _Mode = Mode.New;

        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
                productList.Mode = value;
            }
        }

        public bool CanAccept(object parameter)
        {
            return true;
        }

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            switch (Mode)
            {
                case Mode.New:
                    break;
                case Mode.Update:
                    break;
                case Mode.Delete:
                    break;
                case Mode.ReadOnly:
                    btnNewProduct.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            e.Matches = productList.ApplyFilter(e.Criteria);
        }
    }
}
