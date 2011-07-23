using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroGUI;
using ZeroSales.Pages.Controls;

namespace ZeroSales.Presentation.Controls
{
    public class PaymentInstrumentSelectionViewModel : ViewModelGui
    {
        private ObservableCollection<PaymentInstrument> payments;

        public ObservableCollection<PaymentInstrument> Payments
        {
            get { return payments ?? (payments = new ObservableCollection<PaymentInstrument>(BusinessContext.Instance.Model.PaymentInstruments.Where(p => p.Enable))); }
            set
            {
                if (payments != value)
                {
                    payments = value;
                    OnPropertyChanged("Payments");
                }
            }
        }

        private PaymentInstrument selectedItem;

        public PaymentInstrument SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        private double selectedQuantity;

        public double SelectedQuantity
        {
            get { return selectedQuantity; }
            set
            {
                if (selectedQuantity != value)
                {
                    selectedQuantity = value;
                    OnPropertyChanged("SelectedQuantity");
                }
            }
        }

        private SaleHeader currentSale;
        
        public PaymentInstrumentSelectionViewModel(SaleHeader saleHeader)
            :base(new PaymentInstrumentSelection())
        {
            currentSale = saleHeader;
            SelectedQuantity = saleHeader.SalePaymentHeader.RestToPay;
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (SelectedItem == null)
            {
                Terminal.Instance.Client.ShowDialog(Properties.Resources.MandatoryPeymentInstrument,null, null, MessageBoxButtonEnum.OK);
                ret = false;
            }
            else if(ret && SelectedQuantity == 0 || (!SelectedItem.ChangeEnable.GetValueOrDefault() && SelectedQuantity > currentSale.SalePaymentHeader.RestToPay))
            {
                Terminal.Instance.Client.ShowDialog(Properties.Resources.InvalidAmount,null, null, MessageBoxButtonEnum.OK);
                ret = false;
            }

            return ret;
        }
    }
}
