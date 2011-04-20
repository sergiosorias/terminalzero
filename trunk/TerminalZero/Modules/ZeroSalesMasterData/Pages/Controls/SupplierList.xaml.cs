using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;


namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierGrid.xaml
    /// </summary>
    public partial class SupplierList : ListNavigationControl
    {
        public Entities.MasterDataEntities DataProvider { get; private set; }

        public SupplierList()
        {
            InitializeComponent();
            DataProvider = new Entities.MasterDataEntities();
            InitializeList(suppliersDataGrid, DataProvider.Suppliers);
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            Controls.SupplierDetail detail = new Controls.SupplierDetail(DataProvider, (int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, "Editar proveedor");
            if (ret.HasValue && ret.Value)
            {
                
            }
        }
        
        
    }
}
