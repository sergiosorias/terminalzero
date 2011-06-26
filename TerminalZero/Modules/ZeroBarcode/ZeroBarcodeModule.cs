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
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenBarcodeGeneratorView, OpenCodebarView, null, true));
        }

        public override string[] GetFilesToSend()
        {
            return new string[0];
        }

        public override void Initialize()
        {
            
        }

        #region Handlers
        private void OpenCodebarView(object parameter)
        {
            Terminal.Instance.CurrentClient.ShowView(new BarcodePrintView());
        }
        #endregion
    }
}
