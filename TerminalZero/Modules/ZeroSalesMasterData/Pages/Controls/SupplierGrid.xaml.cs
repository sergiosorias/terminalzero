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
    public partial class SupplierGrid : ZeroGUI.ZeroBasePage
    {
        public Entities.MasterDataEntities DataProvider { get; private set; }
        public SupplierGrid()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
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
                        break;
                    default:
                        break;
                }
                
                DataProvider = new Entities.MasterDataEntities();

                foreach (var item in DataProvider.Suppliers)
                {
                    suppliersDataGrid.Items.Add(item);
                }
            }
        }

        public void RefreshList(Entities.Supplier supplier)
        {
            suppliersDataGrid.Items.Add(supplier);
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            Controls.SupplierDetail detail = new Controls.SupplierDetail(DataProvider, (int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, "Editar proveedor");
            if (ret.HasValue && ret.Value)
            {
                
            }
        }

        internal void ApplyFilter(string criteria)
        {
            suppliersDataGrid.Items.Clear();
            foreach (var item in DataProvider.Suppliers.Where(t => t.Name1.Contains(criteria) || t.Name2.Contains(criteria)))
            {
                suppliersDataGrid.Items.Add(item);
            }
        }
    }
}
