using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroStock.Pages.Controls;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentView.xaml
    /// </summary>
    public partial class DeliveryNoteView : NavigationBasePage
    {
        public DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; private set; }

        public DeliveryNoteView()
        {
            InitializeComponent();
            CommandBar.New += btnNewProduct_Click;
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

        #region IZeroPage Members

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            
            if (ret && ControlMode == ControlMode.Selection)
            {
                SelectedDeliveryDocumentHeader = DeliveryGrid.SelectedItem as DeliveryDocumentHeader;
                ret = (SelectedDeliveryDocumentHeader != null);
                if (!ret)
                {
                    MessageBox.Show("Por favor seleccione un documento!", "Atención", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                
            }

            return ret;
        }

        #endregion

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = DeliveryGrid.ApplyFilter(e.Criteria, dateFilter.SelectedDate); ;
        }

        private void btnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            var det = new DocumentDeliveryDetail();
            bool? res = ZeroMessageBox.Show(det, "Nuevo remito");
            if (res.HasValue && res.Value)
            {
                //DeliveryGrid.AddItem(det.CurrentDocumentDelivery);
            }
        }

        private void dateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DeliveryGrid.ApplyFilter(string.Empty, dateFilter.SelectedDate);
        }
    }
}
