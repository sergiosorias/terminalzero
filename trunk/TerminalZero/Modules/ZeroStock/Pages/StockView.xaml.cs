using System;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for NewStockView.xaml
    /// </summary>
    public partial class StockView : UserControl, IZeroPage
    {
        private readonly ITerminal _terminal;
        private Entities.StockHeader _header;
        private Entities.StockEntities _context;
        readonly int _stockType = -1;
        public StockView(ITerminal terminal, int stockType)
        {
            InitializeComponent();
            _terminal = terminal;
            _stockType = stockType;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                _context = new Entities.StockEntities();
                LoadHeader(_stockType, _context.StockHeaders.Count() > 0 ? _context.GetNextStockHeaderCode() : 1);
                lblStockType.Content = _header.StockType != null && !string.IsNullOrWhiteSpace(_header.StockType.Description) ? _header.StockType.Description : "Stock";
                
                if (IsDeliveryDocumentMandatory())
                {
                    DeliveryDocumentView view = new DeliveryDocumentView(_terminal);
                    view.Mode = Mode.Selection;
                    bool? res = ZeroGUI.ZeroMessageBox.Show(view,Properties.Resources.DeliveryNoteSelection);
                    if (res.HasValue && res.Value)
                    {
                        _header.DeliveryDocumentHeaderCode = view.SelectedDeliveryDocumentHeader.Code;
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.DeliveryNoteMandatoryMsg, Properties.Resources.Fail);
                        TryGoHome();
                    }
                }
            }
        }

        private void TryGoHome()
        {
            ZeroCommonClasses.GlobalObjects.ZeroAction action;
            if (!_terminal.Manager.ExistsAction(ZeroCommonClasses.GlobalObjects.ApplicationActions.Back, out action) 
                || !_terminal.Manager.ExecuteAction(action))
            {
                IsEnabled = false;
            }
        }

        private bool IsDeliveryDocumentMandatory()
        {
            return _stockType == 0;
        }

        private void LoadHeader(int stockType, int code)
        {
            _header = Entities.StockHeader.CreateStockHeader(
                _terminal.TerminalCode,
                code,
                true,
                0,DateTime.Now.Date);
            ZeroCommonClasses.GlobalObjects.ZeroActionParameterBase param =
                _terminal.Session.GetParameter<MembershipUser>();
            if(param!=null)
            {
                _header.UserCode = (Guid)((MembershipUser)param.Value).ProviderUserKey;
            }
            _header.StockType = _context.StockTypes.FirstOrDefault(st => st.Code == stockType);
            _context.AddToStockHeaders(_header);

        }

        #region IZeroPage Members

        private Mode _Mode = Mode.New;
        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                Mode = value;
            }
        }

        public bool CanAccept(object parameter)
        {
            bool ret = true;
            if (_header!=null && _header.StockItems != null && _header.StockItems.Count > 0)
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

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion

        private int _itemsCount;
        private void BarCodeTextBox_BarcodeValidating(object sender, ZeroGUI.Classes.BarCodeValidationEventArgs e)
        {
            if (!(e.Parts[0].IsValid = e.Parts[0].Code > 0 && e.Parts[0].Code <= 12))
                e.Error = Properties.Resources.WrongMonth;
            else if (!(e.Parts[1].IsValid = e.Parts[1].Code > 0 && e.Parts[1].Code <= 31))
                e.Error = Properties.Resources.WrongDay;
            else
            {
                BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name.StartsWith("Prod"));
                if (Part != null)
                {
                    ZeroStock.Entities.Product prod = _context.Products.FirstOrDefault(p => p.MasterCode == Part.Code);
                    if (prod == null)
                    {
                        Part.IsValid = false;
                        e.Error = Properties.Resources.UnexistentProduct;
                    }
                }
            }
            
        }

        private bool SaveData()
        {
            bool ret = false;
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                _terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, ex.ToString());
            }
            return ret;
        }

        private void BarCodeTextBox_BarcodeReceived(object sender, ZeroGUI.Classes.BarCodeEventArgs e)
        {
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
            if (Part != null)
            {
                Entities.Product prod = _context.Products.FirstOrDefault(p => p.MasterCode == Part.Code);
                if (prod != null)
                {
                    if (!prod.Price1Reference.IsLoaded)
                    {
                        prod.Price1Reference.Load();
                    }
                    if (!prod.PriceReference.IsLoaded)
                    {
                        prod.PriceReference.Load();
                    }

                    BarCodePart partQty = e.Parts.FirstOrDefault(p => p.Name == "Cantidad");
                    BarCodePart partDay = e.Parts.FirstOrDefault(p => p.Name == "Día");
                    BarCodePart partMonth = e.Parts.FirstOrDefault(p => p.Name == "Mes");

                    Entities.StockItem item = Entities.StockItem.CreateStockItem(_itemsCount++,
                        _terminal.TerminalCode,
                        _header.Code,
                        true,
                        0,
                        string.Format("{0:00}{1:00}", partMonth.Code, partDay.Code),
                        prod.Code,
                        prod.MasterCode,
                        prod.ByWeight,
                        prod.Price1 != null ? prod.Price1.Value : 0,
                        prod.ByWeight ? partQty.Code : 1
                        );

                    _header.StockItems.Add(item);

                    stockGrid.Add(item);
                }
            }
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

        
    }
}
