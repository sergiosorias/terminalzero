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
        readonly int _stockType = -1;
        public CreateStockView(int stockType)
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
                StockHeader = new StockHeader(BusinessContext.Instance.ModelManager.StockTypes.First(st=>st.Code == _stockType),Terminal.Instance.TerminalCode);
                

                if (BusinessContext.Rules.IsDeliveryDocumentMandatory(_stockType))
                {
                    var view = new DeliveryDocumentView {ControlMode = ControlMode.Selection};
                    bool? res = ZeroMessageBox.Show(view, Properties.Resources.DeliveryNoteSelection);
                    if (res.HasValue && res.Value)
                    {
                        StockHeader.DeliveryDocumentHeader = view.SelectedDeliveryDocumentHeader;
                        StockHeader.TerminalToCode = StockHeader.DeliveryDocumentHeader.TerminalToCode;
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
            
            if (ret && StockHeader != null && StockHeader.HasChanges)
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
                if (!StockHeader.DeliveryDocumentHeaderReference.IsLoaded)
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
                lotBarcode.SetFocus();
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
                BusinessContext.Instance.ModelManager.SaveChanges();
                MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, ex.ToString());
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
