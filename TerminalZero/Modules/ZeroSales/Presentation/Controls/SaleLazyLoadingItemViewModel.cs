using System;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;

namespace ZeroSales.Presentation.Controls
{
    public class SaleLazyLoadingItemViewModel : ViewModelBase
    {
        private SaleItem saleItem;
        private readonly Action<object> deleteAction;

        public SaleItem SaleItem
        {
            get { return saleItem; }
            set
            {
                if (saleItem != value)
                {
                    saleItem = value;
                    OnPropertyChanged("SaleItem");
                }
            }
        }

        private ICommand deleteCommand;

        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new ZeroActionDelegate(deleteAction)); }
            set
            {
                if (deleteCommand != value)
                {
                    deleteCommand = value;
                    OnPropertyChanged("DeleteCommand");
                }
            }
        }

        public SaleLazyLoadingItemViewModel(SaleItem saleItem, Action<object> deleteAction)
        {
            this.saleItem = saleItem;
            this.deleteAction = deleteAction;
        }
    }
}