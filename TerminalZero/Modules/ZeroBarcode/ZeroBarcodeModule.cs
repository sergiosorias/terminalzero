using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ZeroBarcode.Pages.Controls;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroBarcode
{
    public class ZeroBarcodeModule : ZeroModule
    {
        public ZeroBarcodeModule(ITerminal terminal)
            :base(terminal,5,"Generador de Códigos de barras")
        {
            OwnerTerminal.Session.AddAction(new ZeroAction(OwnerTerminal.Session, ActionType.MenuItem, "Configuración@Códigos de barras", OpenCodebarView));
        }

        public override string[] GetFilesToSend()
        {
            return new string[0];
        }

        public override void Init()
        {
            
        }

        #region Handlers
        private void OpenCodebarView()
        {
            ZeroMessageBox.Show(new BarcodeGenerator(), "Generar Código", SizeToContent.WidthAndHeight,
                                ResizeMode.NoResize, MessageBoxButton.OK);
        }
        #endregion
    }
}
