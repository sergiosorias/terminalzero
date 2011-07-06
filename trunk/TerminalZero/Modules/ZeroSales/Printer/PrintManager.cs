using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroBusiness.Entities.Data;

namespace ZeroSales.Printer
{
    class PrintManager
    {
        public static void Print(object obj)
        {
            
        }

        public static void PrintSale(SaleHeader header)
        {
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.CancelPrint();
            foreach (var saleItem in header.SaleItems)
            {
                ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("{0} -- ${1}", saleItem.Product.Name, saleItem.PriceValue));
            }
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine();
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("Total: ${0}", header.PriceSumValue));
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine();
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine("Gracias por su visita");
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.Print();

        }
    }
}
