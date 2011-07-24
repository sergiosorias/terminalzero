using ZeroBarcode.Pages;
using ZeroBarcode.Properties;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroBarcode
{
    public class ZeroBarcodeModule : ZeroModule
    {
        public ZeroBarcodeModule()
            :base(6,Resources.BarcodeModuleDescription)
        {
            
        }

        public override string[] GetFilesToSend()
        {
            return new string[0];
        }

        public override void Initialize()
        {
            
        }

        #region Handlers
        [ZeroAction(Actions.OpenBarcodeGeneratorView, Rules.IsTerminalZero)]
        private void OpenCodebarView(object parameter)
        {
            Terminal.Instance.Client.ShowView(new BarcodePrintView());
        }
        #endregion
    }
}
