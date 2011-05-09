using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroBusiness.Entities.Data;
using ZeroGUI;
using ZeroSales.Pages.Controls;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for SalePaymentView.xaml
    /// </summary>
    public partial class SalePaymentView : NavigationBasePage
    {
        public double TotalValue { get; set; }
        private SaleHeader _sale;
        private SalePaymentHeader SalePaymentHeader
        {
            get { return (SalePaymentHeader) DataContext; }
        }

        public SalePaymentView(SaleHeader sale)
        {
            InitializeComponent();
            _sale = sale;
            TotalValue = sale.PriceSumValue;
        }

        private void NavigationBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            addPaymentInstrument_Click(null, null);
        }

        private void addPaymentInstrument_Click(object sender, RoutedEventArgs e)
        {
            var paymentInstrument = new PaymentInstrumentSelection(_sale);
            paymentInstrument.SelectedQuantity = SalePaymentHeader.RestToPay;
            bool ret = ZeroMessageBox.Show(paymentInstrument, "Forma de pago", ResizeMode.NoResize, MessageBoxButton.OKCancel).GetValueOrDefault();
            if (ret)
            {
                SalePaymentItem newItem = new SalePaymentItem(SalePaymentHeader,paymentInstrument.SelectedItem,paymentInstrument.SelectedQuantity);
                SalePaymentHeader.AddPaymentInstrument(newItem);
                paymentitemsList.AddItem(newItem);
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            return ret && !SalePaymentHeader.NotReady;
        }

        public override bool CanCancel(object parameter)
        {
            DataContext = null;
            return base.CanCancel(parameter);
        }

        private void paymentitemsList_ItemRemoving(object sender, ZeroGUI.Classes.ItemActionEventArgs e)
        {

        }

        private void paymentitemsList_ItemRemoved(object sender, ZeroGUI.Classes.ItemActionEventArgs e)
        {
            SalePaymentHeader.RemovePaymentInstrument((SalePaymentItem)e.Item);
        }
        
    }
}
