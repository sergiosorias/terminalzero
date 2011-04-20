using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using ZeroBarcode.Pages.Controls;
using ZeroBarcode.Properties;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroBarcode
{
    public class ZeroBarcodeModule : ZeroModule
    {
        public ZeroBarcodeModule(ITerminal terminal)
            :base(terminal,6,Resources.BarcodeModuleDescription)
        {
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenBarcodeGeneratorView, OpenCodebarView));
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
            ModuleNotificationEventArgs args = new ModuleNotificationEventArgs
                                                   {ControlToShow = new Pages.BarcodePrintView()};
            OnModuleNotifing(args);
        }
        #endregion
    }
}
