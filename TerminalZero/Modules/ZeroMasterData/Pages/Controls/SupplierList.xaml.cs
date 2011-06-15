using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierGrid.xaml
    /// </summary>
    public partial class SupplierList : ListNavigationControl
    {
        public DataModelManager DataProvider { get; private set; }

        public SupplierList()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new DataModelManager();
                InitializeList(suppliersDataGrid, DataProvider.Suppliers);
            }
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var detail = new SupplierDetail(DataProvider, (int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, "Editar proveedor");
            if (ret.HasValue && ret.Value)
            {
                
            }
        }
        
        
    }
}
