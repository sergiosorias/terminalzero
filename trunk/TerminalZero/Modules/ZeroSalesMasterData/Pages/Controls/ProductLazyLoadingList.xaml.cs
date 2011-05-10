using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGrid.xaml
    /// </summary>
    public partial class ProductLazyLoadingList : LazyLoadingListControl
    {
        public ProductLazyLoadingList()
        {
            InitializeComponent();
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            activeProductcollumn.Visibility = editItemCollum.Visibility = newMode == ControlMode.ReadOnly ? Visibility.Hidden : Visibility.Visible;
            
        }
        
        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            var detail = new ProductDetail(t);

            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.ProductEdit);
            if (ret.HasValue && ret.Value)
            {
               
            }
        }

        private void LazyLoadingListControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                StartListLoad(BusinessContext.Instance.Manager.Products);
            }
        }
        
    }
}
