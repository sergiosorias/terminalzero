using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Classes;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for CreateSaleView.xaml
    /// </summary>
    public partial class CreateSaleView : NavigationBasePage
    {
        public CreateSaleView(int saleType)
        {
            InitializeComponent();
            InitializeComponent();
            _saleType = saleType;
        }

        private SaleHeader _header;
        private DataModelManager _context;
        private string _lot = "";
        readonly int _saleType = -1;
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (_context != null)
                    _context.Dispose();

                _context = new DataModelManager();
                LoadHeader(_saleType, _context.SaleHeaders.Count() > 0 ? _context.GetNextSaleHeaderCode(ZeroCommonClasses.Terminal.Instance.TerminalCode) : 1, ZeroCommonClasses.Terminal.Instance.TerminalCode);
                lblsaleType.Content = _header.SaleType != null &&
                                       !string.IsNullOrWhiteSpace(_header.SaleType.Description)
                                           ? _header.SaleType.Description
                                           : "Venta";

                _header.TerminalToCode = ZeroCommonClasses.Terminal.Instance.TerminalCode;
                DataContext = _header;
            }
        }

        private void LoadHeader(int saleType, int code, int terminalTo)
        {
            _header = SaleHeader.CreateSaleHeader(
                ZeroCommonClasses.Terminal.Instance.TerminalCode,
                code,
                true,
                (int)EntityStatus.New,
                terminalTo,
                DateTime.Now.Date, 
                _saleType,false,0,0,0,false,false);
            ZeroActionParameterBase param =
                ZeroCommonClasses.Terminal.Instance.Session.GetParameter<MembershipUser>();
            if (param != null)
            {
                _header.UserCode = (Guid)((MembershipUser)param.Value).ProviderUserKey;
            }
            _header.SaleType = _context.SaleTypes.FirstOrDefault(st => st.Code == saleType);
            _context.AddToSaleHeaders(_header);

        }

        #region IZeroPage Members

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret && _header.ExistsDataToSave())
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

        public override bool CanCancel(object parameter)
        {
            return CanAccept(parameter);
        }

        #endregion

        private Product validProd;
        private void BarCodeTextBox_BarcodeValidating(object sender, BarCodeValidationEventArgs e)
        {
            validProd = _context.Products.FirstOrDefault(p => p.MasterCode == e.Code);
            if (validProd == null)
            {
                BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
                if (Part != null)
                {
                    string strCode = Part.Code.ToString();
                    validProd = _context.Products.FirstOrDefault(p => p.MasterCode.Equals(strCode));
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
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
            if (Part != null)
            {
                if (validProd != null)
                {
                    BarCodePart partQty = e.Parts.FirstOrDefault(p => p.Name == "Cantidad");
                    currentProd.Text = validProd.Description;
                    CreateResTimer();
                    saleGrid.AddItem(_header.AddNewSaleItem(validProd, partQty.Code, _lot));
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
                var view = new PaymentInstrumentList();
                ret =
                    ZeroMessageBox.Show(view, "Forma de pago", ResizeMode.NoResize, MessageBoxButton.OK).
                        GetValueOrDefault();
                if(ret)
                {
                    object o = view.SelectedItem;
                }
               //MessageBox.Show("Este Módulo es solo de prueba por el momento, los datos no se guardaran", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
               //_context.SaveChanges();
               // currentProd.Text = "";
               // MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
               // ret = true;
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
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            UserControl_Loaded(null, null);
            saleGrid.Clear();
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

        private void saleGrid_Removing(object sender, ItemActionEventArgs e)
        {
            e.Cancel = !ZeroMessageBox.Show(Properties.Resources.ItemDeletingQuestion, Properties.Resources.Delete,
                                           SizeToContent.WidthAndHeight, ResizeMode.NoResize, MessageBoxButton.YesNo).GetValueOrDefault();
        }

        private void saleGrid_ItemRemoved(object sender, ItemActionEventArgs e)
        {
            if (e.Item != null)
            {
                _header.RemoveSaleItem(e.Item as SaleItem);
                lblItemsCount.Text = _header.SaleItems.Count.ToString();
            }
        }
    }
}
