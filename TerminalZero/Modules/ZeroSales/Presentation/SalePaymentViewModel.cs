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
                    Sale.UpdatePrintMode();
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

        private void CustomerSelectionCommand_Finished(object sender, EventArgs e)
        {
            var customer = Terminal.Instance.Session[typeof(Customer)];
            if (customer != null)
            {
                sale.Customer = ((Customer)customer.Value);
                Sale.UpdatePrintMode();
                OnPropertyChanged("CustomerName");
                ViewHeader = "Factura 'A'";
            }
        }
        
        #endregion

        #region Overrides
        public override bool CanAccept(object parameter)
        {
            if(!Payment.Ready)
            {
                //Por favor elija la forma de pago
                return false;
            }
            
            return base.CanAccept(parameter);
        }

        public override bool CanCancel(object parameter)
        {
            Sale.PrintMode = (int) PrintMode.NoTax;
            Sale.CustomerCode = null;
            ContextExtentions.DetachEntities(BusinessContext.Instance.Model, Payment, Payment.SalePaymentItems);
            Sale.SalePaymentHeader = null;
            return base.CanCancel(parameter);
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
