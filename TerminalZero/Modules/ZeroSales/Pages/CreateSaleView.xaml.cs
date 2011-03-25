using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroSales.Entities;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for CreateSaleView.xaml
    /// </summary>
    public partial class CreateSaleView : ZeroBasePage
    {
        public CreateSaleView(ITerminal terminal, int saleType)
        {
            InitializeComponent();
            InitializeComponent();
            _terminal = terminal;
            _saleType = saleType;
        }

        private readonly ITerminal _terminal;
        private SaleHeader _header;
        private SalesEntities _context;
        private string _lot = "";
        readonly int _saleType = -1;
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (_context != null)
                    _context.Dispose();

                _context = new SalesEntities();
                LoadHeader(_saleType, _context.SaleHeaders.Count() > 0 ? _context.GetNextSaleHeaderCode(_terminal) : 1, _terminal.TerminalCode);
                lblsaleType.Content = _header.SaleType != null &&
                                       !string.IsNullOrWhiteSpace(_header.SaleType.Description)
                                           ? _header.SaleType.Description
                                           : "Venta";

                _header.TerminalToCode = _terminal.TerminalCode;
                DataContext = _header;
                saleGrid.Clear();
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

        private void LoadHeader(int saleType, int code, int terminalTo)
        {
            _header = SaleHeader.CreateSaleHeader(
                _terminal.TerminalCode,
                code,
                true,
                (int)EntityStatus.New,
                terminalTo,
                DateTime.Now.Date, 
                0,0,0,0,false,false,false);
            ZeroActionParameterBase param =
                _terminal.Session.GetParameter<MembershipUser>();
            if (param != null)
            {
                _header.UserCode = (Guid)((MembershipUser)param.Value).ProviderUserKey;
            }
            _header.SaleType = _context.SaleTypes.FirstOrDefault(st => st.Code == saleType);
            _context.AddToSaleHeaders(_header);

        }

        #region IZeroPage Members

        private ControlMode _controlMode = ControlMode.New;
        public ControlMode ControlMode
        {
            get
            {
                return _controlMode;
            }
            set
            {
                ControlMode = value;
            }
        }

        public bool CanAccept(object parameter)
        {
            bool ret = true;
            if (_header.ExistsDataToSave())
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
            return CanAccept(parameter);
        }

        #endregion

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
                    BarCodePart partQty = e.Parts.FirstOrDefault(p => p.Name == "Cantidad");
                    currentProd.Text = prod.Description;
                    CreateResTimer();
                    saleGrid.Add(_header.AddNewSaleItem(prod, partQty.Code,_lot));
                    lblItemsCount.Text = _header.SaleItems.Count.ToString();
                    _lot = "";
                    lotBarcode.SetFocus();
                    
                }
            }
        }

        private System.Threading.Timer cleanResTimer = null;

        private void CreateResTimer()
        {
            cleanResTimer = new System.Threading.Timer(cleanResTimer_Elapsed, null, 5000, 5000);
        }

        void cleanResTimer_Elapsed(object o)
        {
            cleanResTimer.Dispose();
            Dispatcher.BeginInvoke(new Update(
                () => { currentProd.Text = ""; }
                ), null);

        }

        private bool SaveData()
        {
            bool ret = false;
            try
            {
               MessageBox.Show("Este Módulo es solo de Prueba por el momento, los datos no se guardaran", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
               //_context.SaveChanges();
               // currentProd.Text = "";
               // MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
               // ret = true;
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

        private void saleGrid_Removing(object sender, RoutedEventArgs e)
        {
            SaleItem item = sender as SaleItem;
            if(item!=null)
            {
                saleGrid.Remove(item);
                _header.RemoveSaleItem(item);
                lblItemsCount.Text = _header.SaleItems.Count.ToString();
            }
        }
    }
}
