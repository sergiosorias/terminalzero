using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Stock;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for DocumentDeliveryDetail.xaml
    /// </summary>
    public partial class DocumentDeliveryDetail : NavigationBasePage
    {
        public DeliveryDocumentHeader CurrentDocumentDelivery { get; private set; }

        public DocumentDeliveryDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                CurrentDocumentDelivery = Context.Instance.Manager.CreateDeliveryDocumentHeader(Terminal.Instance.TerminalCode);
                grid1.DataContext = CurrentDocumentDelivery;
                supplierBox.ItemsSource = Context.Instance.Manager.Suppliers;
                cbTerminals.ItemsSource = Context.Instance.Manager.GetExportTerminal(Terminal.Instance.TerminalCode);
            }
        }

        private void supplierBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentDocumentDelivery.SupplierCode = ((Supplier)supplierBox.SelectedValue).Code;
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret)
            {
                string msg = string.Empty;

                if (!CurrentDocumentDelivery.SupplierCode.HasValue)
                {
                    msg += Properties.Resources.MsgSelectSupplierPlease;
                    ret = false;
                }

                if (cbTerminals.SelectedIndex < 0)
                {
                    msg += "\n" + Properties.Resources.MsgSelectTerminalPlease;
                    ret = false;
                }


                if (ret)
                {
                    try
                    {
                        Context.Instance.Manager.SaveChanges();
                        return ret;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return ret;
        }

        private void supplierBox_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentDocumentDelivery.TerminalToCode = (int) cbTerminals.SelectedValue;
        }
    }
}
