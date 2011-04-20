using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroStock.Entities;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for DocumentDeliveryDetail.xaml
    /// </summary>
    public partial class DocumentDeliveryDetail : NavigationBasePage
    {
        private StockEntities DataProvider;
        private readonly ITerminal _terminal;
        public DeliveryDocumentHeader CurrentDocumentDelivery { get; private set; }

        public DocumentDeliveryDetail(ITerminal terminal)
        {
            InitializeComponent();
            _terminal = terminal;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new StockEntities();

                CurrentDocumentDelivery = DataProvider.CreateDeliveryDocumentHeader(_terminal.TerminalCode);
                grid1.DataContext = CurrentDocumentDelivery;
                supplierBox.ItemsSource = DataProvider.Suppliers;
                cbTerminals.ItemsSource = DataProvider.GetExportTerminal(_terminal.TerminalCode);
            }
        }

        private void supplierBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentDocumentDelivery.SupplierCode = ((Supplier)supplierBox.SelectedValue).Code;
        }

        public override bool CanAccept(object parameter)
        {
            base.CanAccept(parameter);
            string msg = string.Empty;

            if (!CurrentDocumentDelivery.SupplierCode.HasValue)
            {
                msg += Properties.Resources.MsgSelectSupplierPlease;
            }

            if (cbTerminals.SelectedIndex <0)
            {
                msg += "\n"+Properties.Resources.MsgSelectTerminalPlease;
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
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private void supplierBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataProvider != null)
                DataProvider.Dispose();
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentDocumentDelivery.TerminalToCode = (int) cbTerminals.SelectedValue;
        }
    }
}
