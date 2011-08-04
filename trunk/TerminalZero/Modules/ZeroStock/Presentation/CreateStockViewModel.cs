using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroStock.Pages;
using ZeroStock.Properties;

namespace ZeroStock.Presentation
{
    public class CreateStockViewModel : ViewModelGui
    {
        #region Properties

        private StockHeader stockHeader;

        public StockHeader StockHeader
        {
            get { return stockHeader; }
            set
            {
                if (stockHeader != value)
                {
                    stockHeader = value;
                    OnPropertyChanged("StockHeader");
                }
            }
        }

        private ObservableCollection<StockItem> stockItemCollection;

        public ObservableCollection<StockItem> StockItemsCollection
        {
            get
            {
                return stockItemCollection ?? (stockItemCollection = new ObservableCollection<StockItem>());
            }
            set
            {
                if (stockItemCollection != value)
                {
                    stockItemCollection = value;
                    OnPropertyChanged("StockItemsCollection");
                }
            }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        readonly StockType.Types _stockType = StockType.Types.New;

        #endregion

        #region Commands

        private ZeroActionDelegate saveCommand;

        public ZeroActionDelegate SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new ZeroActionDelegate(SaveOperation, CanSaveOperation));
            }
            set
            {
                if (saveCommand != value)
                {
                    saveCommand = value;
                    OnPropertyChanged("SaveCommand");
                }
            }
        }

        private void SaveOperation(object obj)
        {
            if (StockHeader.DeliveryDocumentHeader != null)
            {
                StockHeader.DeliveryDocumentHeader.Used = true;
            }
            BusinessContext.Instance.Model.SaveChanges();
            Exit();
        }

        private bool CanSaveOperation(object obj)
        {
            return StockItemsCollection.Count>0;
        }

        private ZeroActionDelegate cancelCommand;

        public ZeroActionDelegate CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new ZeroActionDelegate(CancelOperation, CanCancelOperation));
            }
            set
            {
                if (cancelCommand != value)
                {
                    cancelCommand = value;
                    OnPropertyChanged("CancelCommand");
                }
            }
        }

        private void CancelOperation(object obj)
        {
            Exit();
        }

        private bool CanCancelOperation(object obj)
        {
            return true;
        }

        #region Lot Barcode Command

        private ICommand lotBarcodeCommand;

        public ICommand BarcodeLotCommand
        {
            get { return lotBarcodeCommand ?? (lotBarcodeCommand = new ZeroActionDelegate(LotBarcodeReceived, LotBarcodeValidate)); }
            set
            {
                if (lotBarcodeCommand != value)
                {
                    lotBarcodeCommand = value;
                    OnPropertyChanged("BarcodeLotCommand");
                }
            }
        }

        private string _lot = string.Empty;

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
            _lot = string.Format("{0:0000}{1:00}{2:00}", e.Parts[1].Code, e.Parts[2].Code, e.Parts[3].Code);
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
            var e = (BarCodeValidationEventArgs)parameter;

            validProd = BusinessContext.Instance.Model.Products.FirstOrDefault(p => p.MasterCode == e.Code);
            if (validProd == null)
            {
                string strCode = e.Parts[1].Code.ToString();
                validProd = BusinessContext.Instance.Model.Products.FirstOrDefault(p => p.MasterCode.Equals(strCode));
                if (validProd == null)
                {
                    e.Parts[1].IsValid = false;
                    e.Error = string.Format(Resources.UnexistentProduct + " - {0}", e.Parts[1].Code);
                }
                else if (validProd.ByWeight && string.IsNullOrEmpty(_lot))
                {
                    e.Parts[1].IsValid = false;
                    e.Error = "Ingrese un Código de lote para este producto";
                }
            }
            Message = e.Error;
            return true;
        }

        private void BarcodeReceived(object parameter)
        {
            var a = parameter as BarCodeEventArgs;
            AddItem(validProd, a.Parts[2].Code, _lot);
            _lot = string.Empty;
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void AddItem(Product product, int qty, string lot)
        {
            StockItemsCollection.Add(StockHeader.AddNewStockItem(product, qty, lot));

            if (StockHeader.DeliveryDocumentHeader != null)
                StockHeader.DeliveryDocumentHeader.AddNewDeliveryDocumentItem(validProd, qty, lot);

            Message = product.Name ?? validProd.ShortDescription ?? validProd.Description;
            saveCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #endregion

        public CreateStockViewModel(StockType.Types stockType)
            : base(new CreateStockView())
        {
            _stockType = stockType;
            StockHeader = new StockHeader(_stockType, Terminal.Instance.Code);

            if (_stockType == StockType.Types.New)
            {
                ViewHeader = "Alta de Stock";
                OpenDeliveryDocumentSelectionView();
            }
            else
            {
                ViewHeader = "Baja de Stock";
                OpenReturnReasonSelectionView();
            }
        }

        private void OpenDeliveryDocumentSelectionView()
        {
            var viewModel = new DeliveryDocumentViewModel(ControlMode.Selection);

            Terminal.Instance.Client.ShowDialog(viewModel.View, Resources.DeliveryNoteSelection,
                (res) =>
                {
                    if (res)
                    {
                        StockHeader.DeliveryDocumentHeader = viewModel.SelectedDeliveryDocumentHeader.Header;
                        StockHeader.TerminalToCode = StockHeader.DeliveryDocumentHeader.TerminalToCode;
                    }
                    else
                    {
                        Terminal.Instance.Client.ShowDialog(Resources.MsgDeliveryNoteMandatory, Resources.Fail, 
                            (o) => Exit());
                    }
                });
        }

        private void OpenReturnReasonSelectionView()
        {
            var viewModel = new ReturnReasonSelectionViewModel();
            Terminal.Instance.Client.ShowDialog(viewModel.View, Resources.DeliveryNoteSelection,
                (res) =>
                {
                    if (res)
                    {
                        StockHeader.ReturnReason = viewModel.ReturnReason;
                        StockHeader.TerminalToCode = viewModel.SelectedTerminal.Code;
                    }
                    else
                    {
                        Terminal.Instance.Client.ShowDialog(Resources.MandatoryReturnReazon, Resources.Fail,
                            (o) => Exit());
                    }
                });
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);

            if (ret && StockHeader != null && StockHeader.HasChanges)
            {
                Terminal.Instance.Client.ShowDialog(Resources.QuestionSaveCurrentData, Resources.Important, (res) =>
                {
                    if (ret)
                        SaveCommand.Execute(null);
                },
                ZeroCommonClasses.GlobalObjects.MessageBoxButtonEnum.YesNo);
            }

            return ret;
        }
    }
}
