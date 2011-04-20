using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroStock.Entities;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for NewStockView.xaml
    /// </summary>
    public partial class CreateStockView : NavigationBasePage
    {
        private readonly ITerminal _terminal;
        private StockHeader _header;
        private StockEntities _context;
        private string _lot = "";
        readonly int _stockType = -1;
        public CreateStockView(ITerminal terminal, int stockType)
        {
            InitializeComponent();
            _terminal = terminal;
            _stockType = stockType;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {

                _context = new StockEntities();

                Guid g = new Guid();
                ZeroActionParameterBase param =
                _terminal.Session.GetParameter<MembershipUser>();
                if (param != null)
                {
                    g = (Guid)((MembershipUser)param.Value).ProviderUserKey;
                }

                _header = _context.CreateStockHeader(_terminal.TerminalCode, _stockType, _context.GetNextStockHeaderCode(), _terminal.TerminalCode, g);

                lblStockType.Content = _header.StockType != null &&
                                       !string.IsNullOrWhiteSpace(_header.StockType.Description)
                                           ? _header.StockType.Description
                                           : "Stock";

                if (_context.IsDeliveryDocumentMandatory(_stockType))
                {
                    var view = new DeliveryDocumentView(_terminal);
                    view.ControlMode = ControlMode.Selection;
                    bool? res = ZeroMessageBox.Show(view, Properties.Resources.DeliveryNoteSelection);
                    if (res.HasValue && res.Value)
                    {
                        _header.DeliveryDocumentHeader =(DeliveryDocumentHeader)_context.GetObjectByKey(view.SelectedDeliveryDocumentHeader.EntityKey);
                        _header.TerminalToCode = _header.DeliveryDocumentHeader.TerminalToCode;
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.MsgDeliveryNoteMandatory, Properties.Resources.Fail);
                        TryGoHome();
                    }
                }

            }
        }

        private void TryGoHome()
        {
            ZeroAction action;
            if (!_terminal.Manager.ExistsAction(ZeroBusiness.Actions.AppHome, out action)
                || !_terminal.Manager.ExecuteAction(action))
            {
                IsEnabled = false;
            }
        }
        
        public override bool CanAccept(object parameter)
        {
            base.CanAccept(parameter);
            bool ret = true;
            if (_header != null && _header.EntityState != EntityState.Unchanged && _header.StockItems != null && _header.StockItems.Count > 0 &&  _header.StockItems.All(si => si.EntityState != EntityState.Unchanged))
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

        private void BarCodeTextBox_BarcodeValidating(object sender, BarCodeValidationEventArgs e)
        {
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name.StartsWith("Prod"));
            if (Part != null)
            {
                Product prod = _context.Products.FirstOrDefault(p => p.MasterCode == Part.Code);
                if (prod == null)
                {
                    Part.IsValid = false;
                    e.Error = string.Format(Properties.Resources.UnexistentProduct+" - {0}",Part.Code);
                }
            }
        }

        private void BarCodeTextBox_BarcodeReceived(object sender, BarCodeEventArgs e)
        {
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
            if (Part != null)
            {
                Product prod = _context.Products.FirstOrDefault(p => p.MasterCode == Part.Code);
                if (prod != null)
                {
                    if (!_header.DeliveryDocumentHeaderReference.IsLoaded)
                    {
                        _header.DeliveryDocumentHeaderReference.Load();
                    }
                    if (!prod.Price1Reference.IsLoaded)
                    {
                        prod.Price1Reference.Load();
                    }
                    if (!prod.PriceReference.IsLoaded)
                    {
                        prod.PriceReference.Load();
                    }

                    BarCodePart partQty = e.Parts.FirstOrDefault(p => p.Name == "Cantidad");

                    stockGrid.AddItem(_header.AddNewStockItem(prod, partQty.Code, _lot));

                    if(_header.DeliveryDocumentHeader!=null)
                        _header.DeliveryDocumentHeader.AddNewDeliveryDocumentItem(prod, partQty.Code,_lot);

                    _lot = "";
                    lotBarcode.SetFocus();
                }
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
                _context.SaveChanges();
                MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                _terminal.Session.Notifier.Log(TraceLevel.Error, ex.ToString());
            }
            return ret;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            TryGoHome();
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
