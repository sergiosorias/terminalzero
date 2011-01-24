using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroStock.Entities;
using ZeroStock.Pages.Controls;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentView.xaml
    /// </summary>
    public partial class DeliveryDocumentView : UserControl, IZeroPage
    {
        private ITerminal Terminal;
        public DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; private set; }

        public DeliveryDocumentView(ITerminal terminal)
        {
            InitializeComponent();
            Terminal = terminal;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
                case Mode.Selection:
                    
                    break;
                default:
                    break;
            }
            // Do not load your data at design time.
             if (!DesignerProperties.GetIsInDesignMode(this))
             {
             	//Load your data here and assign the result to the CollectionViewSource.
                //System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
                //myCollectionViewSource.Source = your data
                 dateFilter.SelectedDateFormat = DatePickerFormat.Short;
                 dateFilter.DisplayDateEnd = DateTime.Now.AddDays(1);
                 dateFilter.SelectedDateChanged+=dateFilter_SelectedDateChanged;
             }
        }

        #region IZeroPage Members

        Mode _Mode = Mode.New;
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

        public bool CanAccept(object parameter)
        {
            bool ret = true;
            if (Mode == Mode.Selection)
            {
                SelectedDeliveryDocumentHeader = DeliveryGrid.SelectedDeliveryDocumentHeader;
                ret = (SelectedDeliveryDocumentHeader != null);
                if (!ret)
                {
                    MessageBox.Show("Por favor seleccione un documento!", "Atención", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                
            }

            return ret;
        }

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            DeliveryGrid.ApplyFilter(e.Criteria);
            e.Matches = DeliveryGrid.deliveryDocumentHeadersDataGrid.Items.Count;
        }

        private void btnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            var det = new DocumentDeliveryDetail(Terminal);
            bool? res = ZeroMessageBox.Show(det, "Nuevo remito");
            if (res.HasValue && res.Value)
            {
                DeliveryGrid.AddDeliveryDocumentHeader(det.CurrentDocumentDelivery);
            }
        }

        private void dateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DeliveryGrid.ApplyFilter(dateFilter.SelectedDate.Value);
        }
    }
}
