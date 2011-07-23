using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;
using ZeroGUI;
using ZeroSales.Pages;
using ZeroSales.Pages.Controls;
using ZeroSales.Presentation.Controls;
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
            CustomerSelectionCommand.Executed += CustomerSelectionCommandExecuted;
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

        public string CustomerName
        {
            get { return Sale.Customer == null ? Resources.FinalCustomer : string.Concat(Sale.Customer.Name1 ?? Sale.Customer.Name2, " - ", Sale.Customer.LegalCode); }
        }
        
        private ObservableCollection<SalePaymentItemViewModel> salePaymentItemsSource;

        public ObservableCollection<SalePaymentItemViewModel> SalePaymentItemsSource
        {
            get { return salePaymentItemsSource ?? (salePaymentItemsSource = new ObservableCollection<SalePaymentItemViewModel>()); }
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
            var paymentInstrument = new PaymentInstrumentSelectionViewModel(Sale);
            Terminal.Instance.Client.ShowDialog(paymentInstrument.View,null,
            (canAddPayment) =>
                {
                    if (canAddPayment)
                    {
                        var payment =
                            Sale.SalePaymentHeader.SalePaymentItems.
                                FirstOrDefault(item =>item.PaymentInstrumentCode == paymentInstrument.SelectedItem.Code);
                        Sale.SalePaymentHeader.Change = paymentInstrument.SelectedQuantity - Sale.SalePaymentHeader.RestToPay;
                        if (payment == null)
                        {
                            var newItem = new SalePaymentItem(Sale.SalePaymentHeader, paymentInstrument.SelectedItem, 
                                Sale.SalePaymentHeader.Change > 0 ? Sale.SalePaymentHeader.RestToPay : paymentInstrument.SelectedQuantity);
                            Sale.SalePaymentHeader.AddPaymentInstrument(newItem);
                            SalePaymentItemsSource.Add(new SalePaymentItemViewModel
                                                            {
                                                                PaymentItem = newItem,
                                                                DeleteCommand = new ZeroActionDelegate(RemovePaymentInstrument)
                                                            });
                            Sale.UpdatePrintMode();
                        }
                        else
                        {
                            payment.Quantity += Sale.SalePaymentHeader.Change > 0 ? Sale.SalePaymentHeader.RestToPay : paymentInstrument.SelectedQuantity;
                        }
                    }
                    addPaymnentInstrumentCommand.RaiseCanExecuteChanged();
                });
        }

        private void RemovePaymentInstrument(object parameter)
        {
            Payment.RemovePaymentInstrument(parameter as SalePaymentItem);
            SalePaymentItemsSource.Remove(SalePaymentItemsSource.FirstOrDefault(item=>item.PaymentItem == parameter));
            Sale.UpdatePrintMode();
        }

        private ZeroActionDelegate alternatePrintModeCommand;

        public ZeroActionDelegate AlternatePrintModeCommand
        {
            get
            {
                return alternatePrintModeCommand ?? (alternatePrintModeCommand = new ZeroActionDelegate((o) => Sale.AlternatePrintMode()));
            }
            set
            {
                if (alternatePrintModeCommand != value)
                {
                    alternatePrintModeCommand = value;
                    OnPropertyChanged("AlternatePrintModeCommand");
                }
            }
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

        private void CustomerSelectionCommandExecuted(object sender, EventArgs e)
        {
            var customer = Terminal.Instance.Session[typeof(Customer)];
            if (customer != null)
            {
                sale.Customer = ((Customer)customer.Value);
                Sale.UpdatePrintMode();
                OnPropertyChanged("CustomerName");
                ViewHeader = Resources.InvoiceTypeA;
            }
        }
        
        #endregion

        #region Overrides
        public override bool CanAccept(object parameter)
        {
            if(!Payment.Ready)
            {
                return false;
            }
            
            return base.CanAccept(parameter);
        }

        public override bool CanCancel(object parameter)
        {
            Sale.PrintMode = (int) PrintMode.NoTax;
            Sale.CustomerCode = null;
            if (Payment != null) ContextExtentions.DetachEntities(BusinessContext.Instance.Model, Payment, Payment.SalePaymentItems);
            Sale.SalePaymentHeader = null;
            return base.CanCancel(parameter);
        }

        public override void Dispose()
        {
            CustomerSelectionCommand.Executed -= CustomerSelectionCommandExecuted;
            base.Dispose();
        }
        #endregion

        

    }
}
