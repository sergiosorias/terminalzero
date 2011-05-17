using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroSales.Printer;

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
            _saleType = saleType;
            CommandBar.Save += btnSave_Click;
            CommandBar.Cancel += btnCancel_Click;
        }

        private SaleHeader _header;
        private string _lot = "";
        readonly int _saleType = -1;
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _header = new SaleHeader(BusinessContext.Instance.ModelManager.SaleTypes.First(st => st.Code == _saleType));
                BusinessContext.Instance.ModelManager.AddToSaleHeaders(_header);
                DataContext = _header;
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret && _header.HasChanges)
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
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
            if (Part != null)
            {
                if (validProd != null)
                {
                    BarCodePart partQty = e.Parts.FirstOrDefault(p => p.Name == "Cantidad");
                    currentProd.Text = validProd.Description ?? validProd.Name;
                    CreateResTimer();
                    saleGrid.AddItem(_header.AddNewSaleItem(validProd, partQty.Code, _lot));
                    lblItemsCount.Text = _header.SaleItems.Count.ToString();
                    _lot = "";
                    lotBarcode.SetFocus();
                    
                }
            }
        }

        private Timer cleanResTimer;

        private void CreateResTimer()
        {
            cleanResTimer = new Timer(cleanResTimer_Elapsed, null, 5000, 5000);
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
                
                var paymentCurrentSale = new SalePaymentHeader(Terminal.Instance.TerminalCode);
                _header.SalePaymentHeader = paymentCurrentSale;
                var salePaymentview = new SalePaymentView(_header) {DataContext = paymentCurrentSale};
                ret = ZeroMessageBox.Show(salePaymentview, "Forma de pago", ResizeMode.NoResize, MessageBoxButton.OKCancel).GetValueOrDefault();
                if (ret)
                {
                    Terminal.Instance.Session[typeof (SaleHeader)] =
                        new ActionParameter<SaleHeader>(true, _header, true);
                    PrintManager.PrintSale(_header);
                    BusinessContext.Instance.ModelManager.SaveChanges();
                    MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    GoHomeOrDisable();
                }
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, ex.ToString());
                GoHomeOrDisable();
            }
            return ret;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(_header.SaleItems!= null && _header.SaleItems.Count>0)
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
