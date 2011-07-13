using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroGUI;
using ZeroSales.Pages;
using ZeroSales.Printer;

namespace ZeroSales.Presentation
{
    public class SaleReportItemViewModel : ViewModelGui
    {
        private ZeroActionDelegate rePrintCommand;

        public ZeroActionDelegate RePrintCommand
        {
            get { return rePrintCommand ?? (rePrintCommand = new ZeroActionDelegate(RePrintSale)); }
            set
            {
                if (rePrintCommand != value)
                {
                    rePrintCommand = value;
                    OnPropertyChanged("RePrintCommand");
                }
            }
        }

        private void RePrintSale(object obj)
        {
            PrintManager.PrintSale(SaleHeader);
        }

        public SaleHeader SaleHeader { get; private set; }

        public SaleReportItemViewModel(SaleHeader saleHeader)
            :base(new SaleReportItemView())
        {
            SaleHeader = saleHeader;
        }
    }
}