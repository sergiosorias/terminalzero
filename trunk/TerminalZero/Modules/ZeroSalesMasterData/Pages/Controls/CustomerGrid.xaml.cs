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
    public partial class CustomerGrid : ZeroBasePage
    {
        public Entities.MasterDataEntities DataProvider { get; private set; }
        public CustomerGrid()
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

                foreach (var item in DataProvider.Customers)
                {
                    customerDataGrid.Items.Add(item);
                }
            }
        }

        public void RefreshList(Entities.Customer supplier)
        {
            customerDataGrid.Items.Add(supplier);
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            Controls.CustomerDetail detail = new Controls.CustomerDetail(DataProvider, (int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, string.Format("Editar cliente", (int)((Button)sender).DataContext));
            if (ret.HasValue && ret.Value)
            {
                
            }
        }

        internal void ApplyFilter(string criteria)
        {
            customerDataGrid.Items.Clear();
            foreach (var item in DataProvider.Customers.Where(t => t.Name1.Contains(criteria) || t.Name2.Contains(criteria)))
            {
                customerDataGrid.Items.Add(item);
            }
        }
    }
}
