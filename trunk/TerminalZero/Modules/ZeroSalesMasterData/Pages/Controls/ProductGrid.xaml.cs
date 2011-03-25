using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGrid.xaml
    /// </summary>
    public partial class ProductGrid : ZeroBasePage
    {
        public MasterDataEntities DataProvider { get; set; }
        public ProductGrid()
        {
            ControlMode = ControlMode.New;
            InitializeComponent();
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
                    editItemCollum.Visibility = Visibility.Hidden;
                    activeProductcollumn.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new MasterDataEntities();
                
                foreach (var item in DataProvider.Products.OrderBy(p => p.Name))
                {
                    productsListView2.Items.Add(item);
                }
            }
        }

        public void AddProduct(Product product)
        {
            productsListView2.Items.Add(product);
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            var detail = new ProductDetail(DataProvider, t);

            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.ProductEdit);
            if (ret.HasValue && ret.Value)
            {
               
            }
        }

        #region IZeroPage Members

        public ControlMode ControlMode { get; set; }

        public bool CanAccept(object parameter)
        {
            return true;
        }

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion

        internal int ApplyFilter(string criteria)
        {
            productsListView2.Items.Clear();
            
            foreach (var item in DataProvider.Products.Where(p =>  p.Name.Contains(criteria) || p.Description.Contains(criteria) || p.ShortDescription.Contains(criteria)))
            {
                productsListView2.Items.Add(item);
            }

            return productsListView2.Items.Count;
        }
                
    }
}
