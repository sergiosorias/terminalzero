using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroStock.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for DocumentDeliveryDetail.xaml
    /// </summary>
    public partial class DocumentDeliveryDetail : UserControl, IZeroPage
    {
        private StockEntities DataProvider;
        private ITerminal Terminal;
        public DeliveryDocumentHeader CurrentDocumentDelivery { get; private set; }

        public DocumentDeliveryDetail(ITerminal terminal)
        {
            InitializeComponent();
            Terminal = terminal;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new StockEntities();
                CurrentDocumentDelivery = Entities.DeliveryDocumentHeader.CreateDeliveryDocumentHeader(
                     Terminal.TerminalCode,
                     DataProvider.DeliveryDocumentHeaders.Count() + 1,
                     true,
                     (short)StockEntities.EntitiesStatus.New,
                     DateTime.Now);
                DataProvider.DeliveryDocumentHeaders.AddObject(CurrentDocumentDelivery);
                grid1.DataContext = CurrentDocumentDelivery;
                supplierBox.ItemsSource = DataProvider.Suppliers;
            }
        }

        private void supplierBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentDocumentDelivery.SupplierCode = ((Supplier)supplierBox.SelectedValue).Code;
        }

        #region IZeroPage Members

        ZeroCommonClasses.Interfaces.Mode _Mode = ZeroCommonClasses.Interfaces.Mode.New;
        public ZeroCommonClasses.Interfaces.Mode Mode
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
            string msg = string.Empty;

            if (!CurrentDocumentDelivery.SupplierCode.HasValue)
            {
                msg += "\nPor favor seleccione un proveedor.";
            }

            if (string.IsNullOrWhiteSpace(msg))
            {
                try
                {
                    DataProvider.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            System.Windows.MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion

        private void supplierBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataProvider != null)
                DataProvider.Dispose();
        }
    }
}
