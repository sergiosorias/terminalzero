using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Classes;


namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for NewStockView.xaml
    /// </summary>
    public partial class CreateStockView : NavigationBasePage
    {
        private StockHeader _header;
        private string _lot = "";
        readonly int _stockType = -1;
        public CreateStockView(int stockType)
        {
            InitializeComponent();
            _stockType = stockType;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _header = new StockHeader(BusinessContext.Instance.ModelManager.StockTypes.First(st=>st.Code == _stockType),Terminal.Instance.TerminalCode);
                Header = _header.ViewTitle;

                if (BusinessContext.Rules.IsDeliveryDocumentMandatory(_stockType))
                {
                    var view = new DeliveryDocumentView();
                    view.ControlMode = ControlMode.Selection;
                    bool? res = ZeroMessageBox.Show(view, Properties.Resources.DeliveryNoteSelection);
                    if (res.HasValue && res.Value)
                    {
                        _header.DeliveryDocumentHeader = (DeliveryDocumentHeader)BusinessContext.Instance.ModelManager.GetObjectByKey(view.SelectedDeliveryDocumentHeader.EntityKey);
                        _header.TerminalToCode = _header.DeliveryDocumentHeader.TerminalToCode;
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.MsgDeliveryNoteMandatory, Properties.Resources.Fail);
                        GoHomeOrDisable();
                    }
                }
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            
            if (ret && _header != null && _header.HasChanges)
            {
                MessageBoxResult quest = MessageBox.Show(Properties.Resources.QuestionSaveCurrentData, Properties.Resources.Important,
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (quest)
                {
                    case MessageBoxResult.Cancel:
                        break;
                    case MessageBoxResult.No:
                        ret = true;
                        break;
                    case MessageBoxResult.None:
                        break;
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Yes:
                        ret = SaveData();

                        break;
                }
            }
            return ret;
        }

        private Product validProd;
        private void BarCodeTextBox_BarcodeValidating(object sender, BarCodeValidationEventArgs e)
        {
            validProd = BusinessContext.Instance.ModelManager.Products.FirstOrDefault(p => p.MasterCode == e.Code);
            if (validProd == null)
            {
                BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
                if (Part != null)
                {
                    string strCode = Part.Code.ToString();
                    validProd = BusinessContext.Instance.ModelManager.Products.FirstOrDefault(p => p.MasterCode.Equals(strCode));
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
                if (!_header.DeliveryDocumentHeaderReference.IsLoaded)
                {
                    _header.DeliveryDocumentHeaderReference.Load();
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

                stockGrid.AddItem(_header.AddNewStockItem(validProd, qty, _lot));

                if (_header.DeliveryDocumentHeader != null)
                    _header.DeliveryDocumentHeader.AddNewDeliveryDocumentItem(validProd, qty, _lot);

                _lot = "";
                lotBarcode.SetFocus();
            }
        }

        private bool SaveData()
        {
            bool ret = false;
            try
            {
                if(_header.DeliveryDocumentHeader!=null)
                {
                    _header.DeliveryDocumentHeader.Used = true;
                }
                BusinessContext.Instance.ModelManager.SaveChanges();
                MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, ex.ToString());
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
            mainBarcode.SetFocus();

        }


    }
}
