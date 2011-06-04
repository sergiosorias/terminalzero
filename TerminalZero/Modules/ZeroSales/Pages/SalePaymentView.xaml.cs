using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroSales.Pages.Controls;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for SalePaymentView.xaml
    /// </summary>
    public partial class SalePaymentView : NavigationBasePage
    {
        public double TotalValue { get; set; }
        public SaleHeader Sale { get; set;}
        private SalePaymentHeader SalePaymentHeader
        {
            get { return (SalePaymentHeader) DataContext; }
        }

        public SalePaymentView(SaleHeader sale)
        {
            InitializeComponent();
            Sale = sale;
            TotalValue = sale.PriceSumValue;
        }

        private void NavigationBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void addPaymentInstrument_Click(object sender, RoutedEventArgs e)
        {
            var paymentInstrument = new PaymentInstrumentSelection(Sale)
                                        {
                                            SelectedQuantity = SalePaymentHeader.RestToPay
                                        };
            bool ret = ZeroMessageBox.Show(paymentInstrument, "Forma de pago", ResizeMode.NoResize, MessageBoxButton.OKCancel).GetValueOrDefault();
            if (ret)
            {
                var newItem = new SalePaymentItem(SalePaymentHeader,paymentInstrument.SelectedItem,paymentInstrument.SelectedQuantity);
                SalePaymentHeader.AddPaymentInstrument(newItem);
                paymentitemsList.AddItem(newItem);
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            return ret && SalePaymentHeader.Ready;
        }

        private void paymentitemsList_ItemRemoving(object sender, ItemActionEventArgs e)
        {

        }

        private void paymentitemsList_ItemRemoved(object sender, ItemActionEventArgs e)
        {
            SalePaymentHeader.RemovePaymentInstrument((SalePaymentItem)e.Item);
        }
    }
}
