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

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentGrid.xaml
    /// </summary>
    public partial class DeliveryDocumentGrid : UserControl
    {
        internal ZeroStock.Entities.StockEntities DataProvider {get;set;}

        internal Entities.DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; private set; }

        public DeliveryDocumentGrid()
        {
            InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
             if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
             {
                 if(DataProvider==null)
                 DataProvider = new Entities.StockEntities();
             	//Load your data here and assign the result to the CollectionViewSource.
                 foreach (var item in DataProvider.DeliveryDocumentHeaders)
                 {
                     deliveryDocumentHeadersDataGrid.Items.Add(item);
                 }
             }
        }

        public void AddDeliveryDocumentHeader(Entities.DeliveryDocumentHeader item)
        {
            deliveryDocumentHeadersDataGrid.Items.Add(item);
        }

        internal int ApplyFilter(string criteria)
        {
            deliveryDocumentHeadersDataGrid.Items.Clear();

            foreach (var item in DataProvider.DeliveryDocumentHeaders.Where(p => criteria == null || 
                criteria == "" || 
                p.Supplier.Name1.Contains(criteria) ||
                p.Supplier.Name2.Contains(criteria) ||
                p.Note.Contains(criteria)))
            {
                deliveryDocumentHeadersDataGrid.Items.Add(item);
            }

            return deliveryDocumentHeadersDataGrid.Items.Count;
        }

        internal int ApplyFilter(DateTime criteria)
        {
            deliveryDocumentHeadersDataGrid.Items.Clear();

            foreach (var item in DataProvider.DeliveryDocumentHeaders)
            {
                if(item.Date.Date == criteria.Date)
                    deliveryDocumentHeadersDataGrid.Items.Add(item);
            }

            return deliveryDocumentHeadersDataGrid.Items.Count;
        }

        private void deliveryDocumentHeadersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDeliveryDocumentHeader = (Entities.DeliveryDocumentHeader)deliveryDocumentHeadersDataGrid.SelectedItem;
        }
    }
}
