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
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("Fecha: {0}",DateTime.Now.Date.ToString("dd/MM/yy")));
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine();
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendColumnsLine("Descripción","cant","P.Unit.","Precio");
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine("-",'-');
            foreach (var saleItem in header.SaleItems)
            {
                ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine(saleItem.Product.Name);
                ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendColumnsLine("", saleItem.Quantity.ToString("0.00"), saleItem.Product.Price1.Value.ToString("0.00"), saleItem.PriceValue.ToString("0.00"));
            }
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine("-", '-');
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("Total: ${0}", header.PriceSumValue).PadRight(ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.MaxColumns));
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine();
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.AppendLine("Gracias por su visita");
            ZeroPrinters.SystemPrinters.Instance.TextOnlyPrinter.Print();

        }
    }
}
