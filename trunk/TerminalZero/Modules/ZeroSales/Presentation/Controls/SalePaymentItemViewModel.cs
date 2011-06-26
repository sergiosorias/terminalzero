using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.MVVMSupport;

namespace ZeroSales.Presentation.Controls
{
    public class SalePaymentItemViewModel : ViewModelBase
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
