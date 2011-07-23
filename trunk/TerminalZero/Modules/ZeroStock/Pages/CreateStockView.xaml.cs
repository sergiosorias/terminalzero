using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroStock.Presentation;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for NewStockView.xaml
    /// </summary>
    public partial class CreateStockView : NavigationBasePage
    {
        private StockHeader StockHeader
        {
            get { return (StockHeader) DataContext; }
            set { DataContext = value; }
        }
        private string _lot = "";
        readonly StockType.Types _stockType = StockType.Types.NotSet;
        public CreateStockView(StockType.Types stockType)
        {
            InitializeComponent();
            _stockType = stockType;
            CommandBar.Save += btnSave_Click;
            CommandBar.Cancel += btnCancel_Click;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode)
            {
                StockHeader = new StockHeader(_stockType,Terminal.Instance.Code);
                
                if (BusinessContext.Rules.IsDeliveryDocumentMandatory(_stockType))
                {
                    var viewModel =
                        new DeliveryDocumentViewModel(new DeliveryDocumentView {ControlMode = ControlMode.Selection});

                    Terminal.Instance.Client.ShowDialog(viewModel.View, Properties.Resources.DeliveryNoteSelection, (res) =>
                    {
                        if (res)
                        {
                            StockHeader.DeliveryDocumentHeader = viewModel.SelectedDeliveryDocumentHeader.Header;
                            StockHeader.TerminalToCode = StockHeader.DeliveryDocumentHeader.TerminalToCode;
                        }
                        else
                        {
                            Terminal.Instance.Client.ShowDialog(Properties.Resources.MsgDeliveryNoteMandatory, Properties.Resources.Fail, (o) => GoHomeOrDisable());
                        }
                    });
                }
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            
            if (ret && StockHeader != null && StockHeader.HasChanges)
            {

                if (ZeroMessageBox.Show(Properties.Resources.QuestionSaveCurrentData, Properties.Resources.Important,
                                        MessageBoxButton.YesNo).GetValueOrDefault())
                {
                    ret = SaveData();
                }
            }
            return ret;
        }

        private Product validProd;
        private void BarCodeTextBox_BarcodeValidating(object sender, BarCodeValidationEventArgs e)
        {
            validProd = BusinessContext.Instance.Model.Products.FirstOrDefault(p => p.MasterCode == e.Code);
            if (validProd == null)
            {
                BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
                if (Part != null)
                {
                    string strCode = Part.Code.ToString();
                    validProd = BusinessContext.Instance.Model.Products.FirstOrDefault(p => p.MasterCode.Equals(strCode));
                    if (validProd == null)
                    {
                        Part.IsValid = false;
                        e.Error = string.Format(Properties.Resources.UnexistentProduct + " - {0}", Part.Code);
                    }
                }
            }
        }

        private void BarCodeTextBox_BarcodeReceived(object sender, BarCodeEventArgs e)
        {
            if (validProd != null)
            {
                if (StockHeader.DeliveryDocumentHeaderCode.HasValue && !StockHeader.DeliveryDocumentHeaderReference.IsLoaded)
                {
                    StockHeader.DeliveryDocumentHeaderReference.Load();
                }
                if (!validProd.Price1Reference.IsLoaded)
                {
                    validProd.Price1Reference.Load();
                }
                if (!validProd.PriceReference.IsLoaded)
                {
                    validProd.PriceReference.Load();
                }

                double qty = 1;
                if(validProd.ByWeight)
                    qty = e.Parts.FirstOrDefault(p => p.Composition.Equals('Q')).Code;

                stockGrid.AddItem(StockHeader.AddNewStockItem(validProd, qty, _lot));

                if (StockHeader.DeliveryDocumentHeader != null)
                    StockHeader.DeliveryDocumentHeader.AddNewDeliveryDocumentItem(validProd, qty, _lot);

                _lot = "";
                lotBarcode.Focus();
            }
        }

        private bool SaveData()
        {
            bool ret = false;
            try
            {
                if(StockHeader.DeliveryDocumentHeader!=null)
                {
                    StockHeader.DeliveryDocumentHeader.Used = true;
                }
                BusinessContext.Instance.Model.SaveChanges();
                ZeroMessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                ZeroMessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK);
                Terminal.Instance.Client.Notifier.Log(TraceLevel.Error, ex.ToString());
            }
            return ret;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            GoHomeOrDisable();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            UserControl_Loaded(null, null);
        }

        private void lotBarcode_BarcodeValidating(object sender, BarCodeValidationEventArgs e)
        {
            if (!(e.Parts[1].IsValid = e.Parts[1].Code >= DateTime.Now.Year))
                e.Error = Properties.Resources.WrongYear;
            else if (!(e.Parts[2].IsValid = e.Parts[2].Code > 0 && e.Parts[2].Code <= 12))
                e.Error = Properties.Resources.WrongMonth;
            else if (!(e.Parts[3].IsValid = e.Parts[3].Code > 0 && e.Parts[3].Code <= 31))
                e.Error = Properties.Resources.WrongDay;

            
        }

        private void lotBarcode_BarcodeReceived(object sender, BarCodeEventArgs e)
        {
            _lot = string.Format("{0:0000}{1:00}{2:00}", e.Parts[1].Code, e.Parts[2].Code, e.Parts[3].Code);
            mainBarcode.Focus();
        }


    }
}
