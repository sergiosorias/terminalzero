using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentGrid.xaml
    /// </summary>
    public partial class DeliveryDocumentList : ZeroGUI.ListNavigationControl
    {
        internal DataModelManager DataProvider { get; set; }
        public bool NotUsedOnly { get; set; }

        public DeliveryDocumentList()
        {
            InitializeComponent();
            if (DataProvider == null)
                DataProvider = new DataModelManager();
            NotUsedOnly = true;
            InitializeList(deliveryDocumentHeadersDataGrid);
        }

        //internal int ApplyFilter(string criteria, DateTime? criteria2)
        //{
        //    deliveryDocumentHeadersDataGrid.Items.Clear();

        //    foreach (var item in DataProvider.DeliveryDocumentHeaders.Where(p => p.Contains(criteria)))
        //    {
        //        if (!criteria2.HasValue || item.Contains(criteria2.Value))
        //        {
        //            if (!item.Used.HasValue)
        //            {
        //                deliveryDocumentHeadersDataGrid.Items.Add(item);
        //            }
        //            else if (!NotUsedOnly || (NotUsedOnly && !item.Used.Value))
        //            {
        //                deliveryDocumentHeadersDataGrid.Items.Add(item);
        //            }
                    
        //        }
        //    }

        //    return deliveryDocumentHeadersDataGrid.Items.Count;
        //}

        private void deliveryDocumentHeadersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = (DeliveryDocumentHeader)deliveryDocumentHeadersDataGrid.SelectedItem;
        }
    }
}
