using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGrid.xaml
    /// </summary>
    public partial class ProductList : ListNavigationControl
    {
        public DataModelManager DataProvider { get; set; }

        public ProductList()
        {
            InitializeComponent();
            
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new DataModelManager();
                InitializeList(productsListView2, DataProvider.Products);
            }
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            activeProductcollumn.Visibility = editItemCollum.Visibility = newMode == ControlMode.ReadOnly ? Visibility.Hidden : Visibility.Visible;
            
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

        
    }
}
