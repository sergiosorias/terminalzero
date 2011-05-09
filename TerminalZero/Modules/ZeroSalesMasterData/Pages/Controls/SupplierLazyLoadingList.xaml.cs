using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.MasterData;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierGrid.xaml
    /// </summary>
    public partial class SupplierLazyLoadingList : LazyLoadingListControl
    {
        public SupplierLazyLoadingList()
        {
            InitializeComponent();
            Loaded += (sender, e) => StartListLoad(Context.Instance.Manager.Suppliers);
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var detail = new SupplierDetail((int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, "Editar proveedor");
            if (ret.HasValue && ret.Value)
            {
                
            }
        }
        
        
    }
}
