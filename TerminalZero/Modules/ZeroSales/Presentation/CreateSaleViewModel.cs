using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroSales.Presentation.Controls;
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
            set
            {
                saleHeader = value;
                OnPropertyChanged("SaleHeader");
            }
        }

        private ObservableCollection<SaleLazyLoadingItemViewModel> productList;

        public ObservableCollection<SaleLazyLoadingItemViewModel> SaleProductList
        {
            get { return productList ?? (productList = new ObservableCollection<SaleLazyLoadingItemViewModel>()); }
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

        #region Commands

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
                var salePaymentviewModel = new SalePaymentViewModel(SaleHeader);
                Terminal.Instance.Client.ShowDialog(salePaymentviewModel.View,null,
                canSave =>
                {
                    if (canSave)
                    {
                        Terminal.Instance.Session[typeof (SaleHeader)] = new ActionParameter<SaleHeader>(true, SaleHeader, true);
                        PrintManager.PrintSale(SaleHeader);
                        BusinessContext.Instance.Model.SaveChanges();
                        CreateSale();
                        productList.Clear();
                        saveCommand.RaiseCanExecuteChanged();
                        Message = Resources.SaveOk;
                    }
                    salePaymentviewModel.Dispose();
                });
                

            }
            catch (Exception ex)
            {
                ZeroMessageBox.Show(ex.Message, Resources.SaveError, MessageBoxButton.OK);
                Terminal.Instance.Client.Notifier.Log(TraceLevel.Error, ex.ToString());
                View.GoHomeOrDisable();
            }

        }

        private bool CanSaveOperation(object parameter)
        {
            return SaleHeader != null && saleHeader.SaleItems != null && SaleHeader.SaleItems.Count > 0;
        }

        #endregion

        #region Cancel Command

        private ZeroActionDelegate cancelCommand;

        public ZeroActionDelegate CancelCommand
        {
            get { return cancelCommand ?? (cancelCommand = new ZeroActionDelegate(CancelOperation, o => SaleHeader != null && SaleHeader.HasChanges)); }
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
            ContextExtentions.DetachEntities(BusinessContext.Instance.Model, SaleHeader, SaleHeader.SaleItems);
            CreateSale();
            productList.Clear();
            saveCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
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
            validProd = BusinessContext.Instance.Model.Products.FirstOrDefault(p => p.MasterCode == e.Code);
            if (validProd == null)
            {
                string strCode = e.Parts[1].Code.ToString();
                validProd = BusinessContext.Instance.Model.Products.FirstOrDefault(p => p.MasterCode.Equals(strCode));
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

        #endregion

        public CreateSaleViewModel(NavigationBasePage view, int saleType)
            : base(view)
        {
            this.saleType = saleType;
            CreateSale();
        }

        #region Overrides
        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret && SaleHeader.HasChanges)
            {
                if (ZeroMessageBox.Show(Resources.QuestionSaveCurrentData, Resources.Important, MessageBoxButton.YesNo).GetValueOrDefault())
                {
                    SaveOperation(null);
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

        private void CreateSale()
        {
            SaleHeader = new SaleHeader(BusinessContext.Instance.Model.SaleTypes.First(st => st.Code == saleType));
        }

        private void AddItem(Product prod, double quantity, string lot)
        {
            Message = prod.Name ?? validProd.ShortDescription ?? validProd.Description;
            SaleProductList.Add(new SaleLazyLoadingItemViewModel(saleHeader.AddNewSaleItem(prod, quantity, lot), DeleteItem));
            saveCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        private void DeleteItem(object parameter)
        {
            if (ZeroMessageBox.Show(Resources.ItemDeletingQuestion, Resources.Delete, MessageBoxButton.YesNo).GetValueOrDefault())
            {
                SaleHeader.RemoveSaleItem(parameter as SaleItem);
                SaleProductList.Remove(SaleProductList.FirstOrDefault(si => si.SaleItem == parameter));
                saveCommand.RaiseCanExecuteChanged();
                CancelCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
