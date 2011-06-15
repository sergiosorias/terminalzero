using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;
using ZeroGUI;
using ZeroSales.Pages;
using ZeroSales.Pages.Controls;
using ZeroSales.Properties;

namespace ZeroSales.Presentation
{
    public class SalePaymentViewModel : ViewModelGui
    {
        public SalePaymentViewModel(SaleHeader sale)
            :base(new SalePaymentView())
        {
            this.sale = sale;
            sale.SalePaymentHeader = new SalePaymentHeader(sale.TerminalToCode);
            ViewHeader = Resources.FinalCustomer;
            CustomerSelectionCommand.Finished += CustomerSelectionCommand_Finished;
            SalePaymentItemsSource.CollectionChanged += (o, e) => { Sale.PrintMode = Sale.CustomerCode != null || Sale.SalePaymentHeader.SalePaymentItems.Any(item => item.PaymentInstrument.PrintModeDefault == 1) ? 1 : 0; };
        }

        #region Properties

        private SaleHeader sale;

        public SaleHeader Sale
        {
            get { return sale; }
            set
            {
                if (sale != value)
                {
                    sale = value;
                    OnPropertyChanged("Sale");
                }
            }
        }
        
        public SalePaymentHeader Payment
        {
            get { return sale.SalePaymentHeader; }
        }

        private string customerName = Resources.FinalCustomer;

        public string CustomerName
        {
            get { return customerName; }
            set
            {
                if (customerName != value)
                {
                    customerName = value;
                    OnPropertyChanged("CustomerName");
                }
            }
        }
        
        private ObservableCollection<SalePaymentItemExtended> salePaymentItemsSource;

        public ObservableCollection<SalePaymentItemExtended> SalePaymentItemsSource
        {
            get { return salePaymentItemsSource ?? (salePaymentItemsSource = new ObservableCollection<SalePaymentItemExtended>()); }
            set
            {
                if (salePaymentItemsSource != value)
                {
                    salePaymentItemsSource = value;
                    OnPropertyChanged("SalePaymentItemsSource");
                }
            }
        }

        #endregion

        #region Commands

        private ZeroActionDelegate addPaymnentInstrumentCommand;

        public ICommand AddPaymentInstrumentCommand
        {
            get { return addPaymnentInstrumentCommand??(addPaymnentInstrumentCommand = new ZeroActionDelegate(AddPaymentInstrument, CanAddPaymentInstrument)); }
            set
            {
                if (addPaymnentInstrumentCommand != value)
                {
                    addPaymnentInstrumentCommand = (ZeroActionDelegate)value;
                    OnPropertyChanged("AddPaymentInstrumentCommand");
                }
            }
        }

        private bool CanAddPaymentInstrument(object parameter)
        {
            return !Sale.PayIsComplete;
        }

        private void AddPaymentInstrument(object parameter)
        {
            var paymentInstrument = new PaymentInstrumentSelection(Sale);
            bool ret = paymentInstrument.ShowInModalWindow();
            if (ret)
            {
                var payment = Sale.SalePaymentHeader.SalePaymentItems.FirstOrDefault(item => item.PaymentInstrumentCode == paymentInstrument.SelectedItem.Code);
                if (payment == null)
                {
                    var newItem = new SalePaymentItem(Sale.SalePaymentHeader, paymentInstrument.SelectedItem, paymentInstrument.SelectedQuantity);
                    Sale.SalePaymentHeader.AddPaymentInstrument(newItem);
                    SalePaymentItemsSource.Add(new SalePaymentItemExtended
                                                   {
                                                       PaymentItem = newItem,
                                                       DeleteCommand = new ZeroActionDelegate(RemovePaymentInstrument)
                                                   });
                }
                else
                {
                    payment.Quantity += paymentInstrument.SelectedQuantity;
                }
            }
            addPaymnentInstrumentCommand.RaiseCanExecuteChanged();
        }

        private void RemovePaymentInstrument(object parameter)
        {
            Payment.RemovePaymentInstrument(parameter as SalePaymentItem);
            SalePaymentItemsSource.Remove(SalePaymentItemsSource.FirstOrDefault(item=>item.PaymentItem == parameter));
        }

        private ZeroAction customerSelectionCommand;

        public ZeroAction CustomerSelectionCommand
        {
            get
            {
                return customerSelectionCommand ??
                       (customerSelectionCommand =
                        Terminal.Instance.Session.Actions[Actions.OpenCustomersSelectionView]);
            }
            set
            {
                if (customerSelectionCommand != value)
                {
                    customerSelectionCommand = value;
                    OnPropertyChanged("CustomerSelectionCommand");
                }
            }
        }

        private void CustomerSelectionCommand_Finished(object sender, EventArgs e)
        {
            var customer = Terminal.Instance.Session[typeof(Customer)];
            if (customer != null)
            {
                CustomerName = string.Concat(((Customer)customer.Value).Name1 ?? ((Customer)customer.Value).Name2, " - ", ((Customer)customer.Value).LegalCode);
                sale.CustomerCode = ((Customer)customer.Value).Code;
                ViewHeader = "Factura 'A'";
            }
        }
        
        #endregion

        #region Overrides
        public override bool CanAccept(object parameter)
        {
            return base.CanAccept(parameter) && Payment.Ready;
        }
        public override void Dispose()
        {
            CustomerSelectionCommand.Finished -= CustomerSelectionCommand_Finished;
            base.Dispose();
        }
        #endregion

        public class SalePaymentItemExtended : ViewModelBase
        {
            private SalePaymentItem paymentItem;

            public SalePaymentItem PaymentItem
            {
                get { return paymentItem; }
                set
                {
                    if (paymentItem != value)
                    {
                        paymentItem = value;
                        OnPropertyChanged("PaymentItem");
                    }
                }
            }

            private ICommand deleteCommand;

            public ICommand DeleteCommand
            {
                get { return deleteCommand; }
                set
                {
                    if (deleteCommand != value)
                    {
                        deleteCommand = value;
                        OnPropertyChanged("DeleteCommand");
                    }
                }
            }
        }

    }
}
