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
    public partial class DeliveryDocumentView : NavigationBasePage
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
                SelectedDeliveryDocumentHeader = DeliveryGrid.SelectedItem as DeliveryDocumentHeader;
                ret = (SelectedDeliveryDocumentHeader != null);
                if (!ret)
                {
                    MessageBox.Show("¡Por favor seleccione un documento!", "Atención", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                
            }

            return ret;
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = DeliveryGrid.ApplyFilter(e.Criteria, dateFilter.SelectedDate);
        }

        private void btnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            var det = new DocumentDeliveryDetail(Terminal);
            bool? res = ZeroMessageBox.Show(det, Properties.Resources.NewDeliveryNote);
            if (res.HasValue && res.Value)
            {
                DeliveryGrid.AddItem(det.CurrentDocumentDelivery);
            }
        }

        private void dateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DeliveryGrid.ApplyFilter(string.Empty, dateFilter.SelectedDate);
        }

        private void DeliveryGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlMode == ControlMode.Selection)
            {
                DeliveryGrid.ApplyFilter(string.Empty, DateTime.Now.Date);
            }
        }
    }
}
