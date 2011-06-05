using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroSales.Pages;
using ZeroSales.Printer;
using ZeroSales.Properties;

namespace ZeroSales.Presentation
{
    public class CreateSaleViewModel : ViewModelGui
    {
        #region Properties

        public object ViewTitle
        {
            get
            {
                return !string.IsNullOrWhiteSpace(saleHeader.SaleType.Description) ? saleHeader.SaleType.Description : "Venta";
            }
        }

        private readonly int saleType;

        private SaleHeader saleHeader;

        public SaleHeader SaleHeader
        {
            get { return saleHeader; }
            set { saleHeader = value;
                OnPropertyChanged("SaleHeader");
            }
        }

        private ObservableCollection<SaleItemExtended> productList;

        public ObservableCollection<SaleItemExtended> SaleProductList
        {
            get { return productList??(productList = new ObservableCollection<SaleItemExtended>()); }
            set
            {
                if (productList != value)
                {
                    productList = value;
                    OnPropertyChanged("SaleProductList");
                }
            }
        }

        private string messsage;

        public string Message
        {
            get { return messsage; }
            set
            {
                messsage = value;
                OnPropertyChanged("Message");
                
            }
        }

        #endregion

        #region Save Command
        private ZeroActionDelegate saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new ZeroActionDelegate(SaveOperation, o => CanSaveOperation(o)));
            }
            private set
            {
                if (saveCommand != value)
                {
                    saveCommand = (ZeroActionDelegate)value;
                    OnPropertyChanged("SaveCommand");
                }
            }
        }

        private void SaveOperation(object parameter)
        {
            try
            {
                var salePaymentview = new SalePaymentViewModel(SaleHeader);
                if (salePaymentview.View.ShowInModalWindow())
                {
                    Terminal.Instance.Session[typeof(SaleHeader)] =
                        new ActionParameter<SaleHeader>(true, SaleHeader, true);
                    PrintManager.PrintSale(SaleHeader);
                    BusinessContext.Instance.ModelManager.AddToSaleHeaders(SaleHeader);
                    BusinessContext.Instance.ModelManager.SaveChanges();
                    MessageBox.Show(Resources.SaveOk, Resources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.SaveError, MessageBoxButton.OK, MessageBoxImage.Error);
                Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, ex.ToString());
                View.GoHomeOrDisable();
            }

        }

        private bool CanSaveOperation(object parameter)
        {
            return SaleHeader != null && saleHeader.SaleItems != null && SaleHeader.SaleItems.Count>0;
        }

        #endregion

        #region Cancel Command

        private ICommand cancelCommand;

        public ICommand CancelCommand
        {
            get { return cancelCommand ??(cancelCommand = new ZeroActionDelegate(CancelOperation)); }
            set
            {
                if (cancelCommand != value)
                {
                    cancelCommand = value;
                    OnPropertyChanged("CancelCommand");
                }
            }
        }

        private void CancelOperation(object parameter)
        {
            SaleHeader = new SaleHeader(BusinessContext.Instance.ModelManager.SaleTypes.First(st => st.Code == saleType));
            productList.Clear();
            saveCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Lot Barcode Command

        private ICommand lotBarcodeCommand;

        public ICommand LotBarcodeProductCommand
        {
            get { return lotBarcodeCommand ?? (lotBarcodeCommand = new ZeroActionDelegate(LotBarcodeReceived, LotBarcodeValidate)); }
            set
            {
                if (lotBarcodeCommand != value)
                {
                    lotBarcodeCommand = value;
                    OnPropertyChanged("LotBarcodeProductCommand");
                }
            }
        }

        private bool LotBarcodeValidate(object parameter)
        {
            var e = parameter as BarCodeValidationEventArgs;
            if (!(e.Parts[1].IsValid = e.Parts[1].Code >= DateTime.Now.Year))
                Message = Resources.WrongYear;
            else if (!(e.Parts[2].IsValid = e.Parts[2].Code > 0 && e.Parts[2].Code <= 12))
                Message = Resources.WrongMonth;
            else if (!(e.Parts[3].IsValid = e.Parts[3].Code > 0 && e.Parts[3].Code <= 31))
                Message = Resources.WrongDay;
            return true;
        }

        private void LotBarcodeReceived(object parameter)
        {
            var e = parameter as BarCodeEventArgs;
            string lot = string.Format("{0:0000}{1:00}{2:00}", e.Parts[1].Code, e.Parts[2].Code, e.Parts[3].Code);
        }

        #endregion

        #region Barcode Command

        private ICommand barcodeCommand;

        public ICommand BarcodeProductCommand
        {
            get { return barcodeCommand ?? (barcodeCommand = new ZeroActionDelegate(BarcodeReceived, BarcodeValidate)); }
            set
            {
                if (barcodeCommand != value)
                {
                    barcodeCommand = value;
                    OnPropertyChanged("BarcodeProductCommand");
                }
            }
        }

        private Product validProd;
        private bool BarcodeValidate(object parameter)
        {
            var e = parameter as BarCodeValidationEventArgs;
            validProd = BusinessContext.Instance.ModelManager.Products.FirstOrDefault(p => p.MasterCode == e.Code);
            if (validProd == null)
            {
                string strCode = e.Parts[1].Code.ToString();
                validProd = BusinessContext.Instance.ModelManager.Products.FirstOrDefault(p => p.MasterCode.Equals(strCode));
                if (validProd == null)
                {
                    e.Parts[1].IsValid = false;
                    Message = string.Format(Resources.UnexistentProduct + " - {0}", e.Parts[1].Code);
                }
            }
            return true;
        }

        private void BarcodeReceived(object parameter)
        {
            var a = parameter as BarCodeEventArgs;
            AddItem(validProd, a.Parts[2].Code, "");
        }

        #endregion

        public CreateSaleViewModel(NavigationBasePage view, int saleType)
            :base(view)
        {
            this.saleType = saleType;
            SaleHeader = new SaleHeader(BusinessContext.Instance.ModelManager.SaleTypes.First(st => st.Code == saleType));
            
        }
        
        #region Overrides
        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret && SaleHeader.HasChanges)
            {
                MessageBoxResult quest = MessageBox.Show(Resources.QuestionSaveCurrentData, Resources.Important,
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (quest)
                {
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Yes:
                        SaveOperation(null);
                        break;
                    default:
                        break;
                }
            }

            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            SaleProductList.Clear();
            return base.CanCancel(parameter);
        }
        #endregion

        private void AddItem(Product prod, double quantity, string lot)
        {
            Message = prod.Name ?? validProd.ShortDescription ?? validProd.Description;
            SaleProductList.Add(new SaleItemExtended(saleHeader.AddNewSaleItem(prod, quantity, lot), DeleteItem));
            saveCommand.RaiseCanExecuteChanged();
        }

        private void DeleteItem(object parameter)
        {
            if (ZeroMessageBox.Show(Resources.ItemDeletingQuestion, Resources.Delete, MessageBoxButton.YesNo).GetValueOrDefault())
            {
                SaleHeader.RemoveSaleItem(parameter as SaleItem);
                SaleProductList.Remove(SaleProductList.FirstOrDefault(si => si.SaleItem == parameter));
            }
        }

        public class SaleItemExtended : ViewModelBase
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
                get { return deleteCommand??(deleteCommand = new ZeroActionDelegate(deleteAction)); }
                set
                {
                    if (deleteCommand != value)
                    {
                        deleteCommand = value;
                        OnPropertyChanged("DeleteCommand");
                    }
                }
            }

            public SaleItemExtended(SaleItem saleItem, Action<object> deleteAction)
            {
                this.saleItem = saleItem;
                this.deleteAction = deleteAction;
            }
        }
        
    }

    
}
