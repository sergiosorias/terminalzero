using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGrid.xaml
    /// </summary>
    public partial class ProductGrid : UserControl, IZeroPage
    {
        public Entities.MasterDataEntities DataProvider { get; set; }
        public ProductGrid()
        {
            InitializeComponent();
            
        }

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
                    editItemCollum.Visibility = Visibility.Hidden;
                    activeProductcollumn.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new Entities.MasterDataEntities();
                
                foreach (var item in DataProvider.Products.OrderBy(p => p.Name))
                {
                    productsListView2.Items.Add(item);
                }
            }
        }

        public void AddProduct(Entities.Product product)
        {
            productsListView2.Items.Add(product);
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            int t = (int)((Button)sender).DataContext;
            Controls.ProductDetail detail = new Controls.ProductDetail(DataProvider, t);

            bool? ret = ZeroMessageBox.Show(detail, "Editar producto");
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

        internal int ApplyFilter(string criteria)
        {
            productsListView2.Items.Clear();

            foreach (var item in DataProvider.Products.Where(p => p.Name.Contains(criteria) || p.Description.Contains(criteria) || p.ShortDescription.Contains(criteria)))
            {
                productsListView2.Items.Add(item);
            }

            return productsListView2.Items.Count;
        }
                
    }
}
