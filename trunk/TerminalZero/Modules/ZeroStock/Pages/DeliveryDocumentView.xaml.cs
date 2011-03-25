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
    public partial class DeliveryDocumentView : ZeroBasePage
    {
        private readonly ITerminal Terminal;
        public DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; private set; }

        public DeliveryDocumentView(ITerminal terminal)
        {
            ControlMode = ControlMode.New;
            InitializeComponent();
            Terminal = terminal;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
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

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = DeliveryGrid.ApplyFilter(e.Criteria, dateFilter.SelectedDate); ;
        }

        private void btnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            var det = new DocumentDeliveryDetail(Terminal);
            bool? res = ZeroMessageBox.Show(det, Properties.Resources.NewDeliveryNote);
            if (res.HasValue && res.Value)
            {
                DeliveryGrid.AddDeliveryDocumentHeader(det.CurrentDocumentDelivery);
            }
        }

        private void dateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DeliveryGrid.ApplyFilter(string.Empty, dateFilter.SelectedDate); ;
        }

        private void DeliveryGrid_Loaded(object sender, RoutedEventArgs e)
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
                    DeliveryGrid.ApplyFilter(string.Empty, DateTime.Now.Date);
                    break;
            }
        }
    }
}
