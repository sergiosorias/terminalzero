using System;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroStock.Pages.Controls;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentView.xaml
    /// </summary>
    public partial class DeliveryNoteView : ZeroGUI.ZeroBasePage
    {
        private ITerminal Terminal;
        public Entities.DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; private set; }

        public DeliveryNoteView(ITerminal terminal)
        {
            InitializeComponent();
            Terminal = terminal;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
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
                case ControlMode.Selection:
                    
                    break;
                default:
                    break;
            }
            // Do not load your data at design time.
             if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
             {
             	//Load your data here and assign the result to the CollectionViewSource.
                //System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
                //myCollectionViewSource.Source = your data
                 dateFilter.SelectedDateFormat = DatePickerFormat.Short;
                 dateFilter.DisplayDateEnd = DateTime.Now.AddDays(1);
                 dateFilter.SelectedDateChanged+=new EventHandler<SelectionChangedEventArgs>(dateFilter_SelectedDateChanged);
             }
        }

        #region IZeroPage Members

        public override bool CanAccept(object parameter)
        {
            base.CanAccept(parameter);
            bool ret = true;
            if (ControlMode == ControlMode.Selection)
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

        #endregion

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            e.Matches = DeliveryGrid.ApplyFilter(e.Criteria, dateFilter.SelectedDate); ;
        }

        private void btnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            DocumentDeliveryDetail det = new DocumentDeliveryDetail(Terminal);
            bool? res = ZeroGUI.ZeroMessageBox.Show(det, "Nuevo remito");
            if (res.HasValue && res.Value)
            {
                DeliveryGrid.AddDeliveryDocumentHeader(det.CurrentDocumentDelivery);
            }
        }

        private void dateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DeliveryGrid.ApplyFilter(string.Empty, dateFilter.SelectedDate);
        }
    }
}
