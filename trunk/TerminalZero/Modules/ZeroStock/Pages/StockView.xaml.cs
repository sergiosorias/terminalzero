﻿using System;
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
    public partial class StockView : UserControl, IZeroPage
    {
        ITerminal Terminal;
        Entities.StockHeader Header;
        Entities.StockEntities Context;
        int StockType = -1;
        public StockView(ITerminal terminal, int stockType)
        {
            InitializeComponent();
            Terminal = terminal;
            StockType = stockType;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                Context = new Entities.StockEntities();
                LoadHeader(StockType, Context.StockHeaders.Count() > 0 ? Context.GetNextStockHeaderCode() : 1);
                lblStockType.Content = Header.StockType != null && !string.IsNullOrWhiteSpace(Header.StockType.Description) ? Header.StockType.Description : "Stock";
                
                if (IsDeliveryDocumentMandatory())
                {
                    DeliveryDocumentView view = new DeliveryDocumentView(Terminal);
                    view.Mode = ZeroCommonClasses.Interfaces.Mode.Selection;
                    bool? res = ZeroGUI.ZeroMessageBox.Show(view);
                    if (res.HasValue && res.Value)
                    {
                        Header.DeliveryDocumentHeaderCode = view.SelectedDeliveryDocumentHeader.Code;
                    }
                    else
                    {
                        MessageBox.Show("Tiene que seleccionar un Documento para poder continuar!");
                        TryGoHome();
                    }
                }
            }
        }

        private void TryGoHome()
        {
            ZeroCommonClasses.GlobalObjects.ZeroAction action;
            if (Terminal.Manager.ExistsAction(ZeroCommonClasses.GlobalObjects.ApplicationActions.Back, out action))
            {
                action.Execute(null);
            }
            else
            {
                this.IsEnabled = false;
            }
        }

        private bool IsDeliveryDocumentMandatory()
        {
            return StockType == 0;
        }

        private void LoadHeader(int stockType, int code)
        {
            Header = Entities.StockHeader.CreateStockHeader(
                Terminal.TerminalCode,
                code,
                true,
                0,DateTime.Now.Date);
            Header.StockType = Context.StockTypes.FirstOrDefault(st => st.Code == stockType);
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
            bool ret = true;
            if (Header!=null && Header.StockItems != null && Header.StockItems.Count > 0)
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
                        ret = SaveData();

                        break;
                }
            }
            return ret;
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion

        private int itemsCount = 0;
        private void BarCodeTextBox_BarcodeValidating(object sender, ZeroGUI.Classes.BarCodeValidationEventArgs e)
        {
            if (!(e.Parts[0].IsValid = e.Parts[0].Code > 0 && e.Parts[0].Code <= 12))
                e.Error = "Mes incorrecto";
            else if (!(e.Parts[1].IsValid = e.Parts[1].Code > 0 && e.Parts[1].Code <= 31))
                e.Error = "Día incorrecto";
            else
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
            
        }

        private bool SaveData()
        {
            bool ret = false;
            try
            {
                Context.SaveChanges();
                MessageBox.Show("Datos Guardados", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
                MessageBox.Show(ex.Message, "Error al guardar", MessageBoxButton.OK, MessageBoxImage.Error);
                Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, ex.ToString());
            }
            return ret;
        }

        private void BarCodeTextBox_BarcodeReceived(object sender, ZeroGUI.Classes.BarCodeEventArgs e)
        {
            BarCodePart Part = e.Parts.FirstOrDefault(p => p.Name == "Producto");
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
                        prod.Price1 != null ? prod.Price1.Value : 0,
                        prod.ByWeight ? PartQty.Code : 1
                        );

                    Header.StockItems.Add(item);

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