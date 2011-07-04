using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.MVVMSupport;
using ZeroSales.Pages;

namespace ZeroSales.Presentation
{
    public class SaleReportItemViewModel : ZeroGUI.ViewModelGui
    {
        public SaleHeader SaleHeader { get; private set; }
        public SaleReportItemViewModel(SaleHeader saleHeader)
            :base(new SaleReportItemView())
        {
            SaleHeader = saleHeader;
        }
    }
}