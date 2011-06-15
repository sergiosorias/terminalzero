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
    public partial class CustomerList : ListNavigationControl
    {
        public DataModelManager DataProvider { get; private set; }
        public CustomerList()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new DataModelManager();
                InitializeList(customerDataGrid, DataProvider.Customers);
            }
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var detail = new CustomerDetail(DataProvider, (int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, string.Format("Editar cliente {0}", (int)((Button)sender).DataContext));
            if (ret.HasValue && ret.Value)
            {
                
            }
        }
    }
}
