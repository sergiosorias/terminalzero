using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroStock.Pages.Controls;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentView.xaml
    /// </summary>
    public partial class DeliveryDocumentView : NavigationBasePage
    {
        public DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; private set; }
        
        private ICommand openNewDocumentCommand;
        public ICommand OpenNewDocumentCommand
        {
            get { return openNewDocumentCommand ?? (openNewDocumentCommand = new ZeroActionDelegate(OpenNewDocument)); }
        }

        public DeliveryDocumentView()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
             // Do not load your data at design time.
            if (!IsInDesignMode)
            {
             	dateFilter.SelectedDateFormat = DatePickerFormat.Short;
                dateFilter.DisplayDateEnd = DateTime.Now.AddDays(1);
                dateFilter.SelectedDateChanged+=dateFilter_SelectedDateChanged;
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret && ControlMode == ControlMode.Selection)
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

        private void OpenNewDocument(object parameter)
        {
            var det = new DocumentDeliveryDetail();
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

        private void DeliveryGrid_ItemsLoaded(object sender, EventArgs e)
        {
            if (ControlMode == ControlMode.Selection)
            {
                DeliveryGrid.ApplyFilter(string.Empty, DateTime.Now.Date);
            }
        }
        
    }
}
