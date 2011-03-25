using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroStock.Entities;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentGrid.xaml
    /// </summary>
    public partial class DeliveryDocumentGrid : ZeroGUI.ZeroBasePage
    {
        internal StockEntities DataProvider {get;set;}
        public bool NotUsedOnly { get; set; }

        internal DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; private set; }

        public DeliveryDocumentGrid()
        {
            InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
             if (!DesignerProperties.GetIsInDesignMode(this))
             {
                 if(DataProvider==null)
                 DataProvider = new StockEntities();
                 NotUsedOnly = true;
             }
        }

        public void AddDeliveryDocumentHeader(DeliveryDocumentHeader item)
        {
            deliveryDocumentHeadersDataGrid.Items.Add(item);
        }

        internal int ApplyFilter(string criteria, DateTime? criteria2)
        {
            deliveryDocumentHeadersDataGrid.Items.Clear();

            foreach (var item in DataProvider.DeliveryDocumentHeaders.Where(p => string.IsNullOrEmpty(criteria) ||
                p.Supplier.Name1.Contains(criteria) ||
                p.Supplier.Name2.Contains(criteria) ||
                p.Note.Contains(criteria)))
            {
                if (!criteria2.HasValue || item.Date.Date == criteria2.Value.Date)
                {
                    if (!item.Used.HasValue)
                    {
                        deliveryDocumentHeadersDataGrid.Items.Add(item);
                    }
                    else if (!NotUsedOnly || (NotUsedOnly && !item.Used.Value))
                    {
                        deliveryDocumentHeadersDataGrid.Items.Add(item);
                    }
                    
                }
            }

            return deliveryDocumentHeadersDataGrid.Items.Count;
        }

        private void deliveryDocumentHeadersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDeliveryDocumentHeader = (DeliveryDocumentHeader)deliveryDocumentHeadersDataGrid.SelectedItem;
        }
    }
}
