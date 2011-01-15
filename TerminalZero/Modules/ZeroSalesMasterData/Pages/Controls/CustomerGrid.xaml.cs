using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierGrid.xaml
    /// </summary>
    public partial class CustomerGrid : UserControl, IZeroPage
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
                switch (Mode)
                {
                    case Mode.New:
                        break;
                    case Mode.Update:
                        break;
                    case Mode.Delete:
                        break;
                    case Mode.ReadOnly:
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
            bool? ret = ZeroMessageBox.Show(detail);
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

        public bool CanAccept()
        {
            return true;
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion

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
