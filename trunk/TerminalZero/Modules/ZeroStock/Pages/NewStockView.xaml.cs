using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.GlobalObjects.Barcode;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for NewStockView.xaml
    /// </summary>
    public partial class NewStockView : UserControl, IZeroPage
    {
        ITerminal Terminal;
        Entities.StockHeader Header;
        Entities.StockEntities Context;
        public NewStockView(ITerminal terminal)
        {
            InitializeComponent();
            Terminal = terminal;
            Context = new Entities.StockEntities();
            LoadHeader();
        }

        private void LoadHeader()
        {
            Header = Entities.StockHeader.CreateStockHeader(
                Terminal.TerminalCode,
                0,
                true,
                0);

            Context.AddToStockHeaders(Header);


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

        public bool CanAccept()
        {
            bool ret = false;
            if (Header.StockItems != null && Header.StockItems.Count > 0)
            {
                MessageBoxResult quest = MessageBox.Show("¿Desea guardar los datos ingresados?", "Importante",
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
                        try
                        {
                            Context.SaveChanges();
                            ret = true;
                        }
                        catch (Exception ex)
                        {
                            ret = false;
                            MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        
                        break;
                }
            }
            return false;
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion

        private int itemsCount = 0;
        private void BarCodeTextBox_BarcodeValidating(object sender, ZeroGUI.Classes.BarCodeValidationEventArgs e)
        {
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name.StartsWith("Prod"));
            if (Part != null)
            {
                ZeroStock.Entities.Product prod = Context.Products.FirstOrDefault(p => p.MasterCode == Part.Code);
                if (prod == null)
                {
                    Part.IsValid = false;
                    e.Error = "Producto Inexistente";
                }
            }
        }

        private void BarCodeTextBox_BarcodeReceived(object sender, ZeroGUI.Classes.BarCodeEventArgs e)
        {
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto" );
            if (Part != null)
            {
                ZeroStock.Entities.Product prod = Context.Products.FirstOrDefault(p => p.MasterCode == Part.Code);
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
                    
                    BarCodePart PartQty = e.Parts.FirstOrDefault(p => p.Name == "Cantidad");
                    BarCodePart PartDay = e.Parts.FirstOrDefault(p => p.Name == "Día");
                    BarCodePart PartMonth = e.Parts.FirstOrDefault(p => p.Name == "Mes");

                    Entities.StockItem item = Entities.StockItem.CreateStockItem(itemsCount++,
                        Terminal.TerminalCode,
                        Header.Code,
                        true,
                        0,
                        string.Format("{0:00}{1:00}", PartMonth.Code, PartDay.Code),
                        prod.Code,
                        prod.MasterCode,
                        prod.ByWeight,
                        prod.Price1!=null? prod.Price1.Value:0,
                        prod.ByWeight?PartQty.Code:1
                        );

                    Header.StockItems.Add(item);

                    stockGrid.Add(item);
                }
            }
        }
    }
}
