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
    public partial class DeliveryDocumentGrid : UserControl
    {
        internal StockEntities DataProvider {get;set;}

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
             	//Load your data here and assign the result to the CollectionViewSource.
                 foreach (var item in DataProvider.DeliveryDocumentHeaders)
                 {
                     deliveryDocumentHeadersDataGrid.Items.Add(item);
                 }
             }
        }

        public void AddDeliveryDocumentHeader(DeliveryDocumentHeader item)
        {
            deliveryDocumentHeadersDataGrid.Items.Add(item);
        }

        internal int ApplyFilter(string criteria, DateTime? criteria2)
        {
            deliveryDocumentHeadersDataGrid.Items.Clear();

            foreach (var item in DataProvider.DeliveryDocumentHeaders.Where(p => criteria == null ||
                criteria == "" ||
                p.Supplier.Name1.Contains(criteria) ||
                p.Supplier.Name2.Contains(criteria) ||
                p.Note.Contains(criteria)))
            {
                if ((!criteria2.HasValue || item.Date.Date == criteria2.Value.Date))
                {
                    deliveryDocumentHeadersDataGrid.Items.Add(item);
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
