using System;
using ZeroBusiness.Entities.Data;
using ZeroPrinters;

namespace ZeroSales.Printer
{
    internal class PrintManager
    {
        public static void Print(object obj)
        {
            
        }

        public static void PrintSale(SaleHeader header)
        {
            //TerminalPrinters.Instance.DriverTextOnlyPrinter.Open();
            TerminalPrinters.Instance.TextOnlyPrinter.CancelPrint();
            TerminalPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("Fecha: {0}   Hora: {1}", DateTime.Now.Date.ToString("dd/MM/yy"), DateTime.Now.ToString("hh:mm:ss")));
            TerminalPrinters.Instance.TextOnlyPrinter.AppendColumnsLine("Descripcion    ","Cant.   ","P.Unit.  ","Precio");
            TerminalPrinters.Instance.TextOnlyPrinter.AppendLine("=",'=');
            foreach (var saleItem in header.SaleItems)
            {
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine(saleItem.Product.Name);
                TerminalPrinters.Instance.TextOnlyPrinter.AppendColumnsLine("", saleItem.Quantity.ToString("0.00"), saleItem.Product.Price1.Value.ToString("0.00"), saleItem.PriceValue.ToString("0.00"));
            }
            TerminalPrinters.Instance.TextOnlyPrinter.AppendLine("=", '=');
            TerminalPrinters.Instance.TextOnlyPrinter.AppendLine();
            TerminalPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("Total: ${0}", header.PriceSumValue));
            TerminalPrinters.Instance.TextOnlyPrinter.AppendLine();
            TerminalPrinters.Instance.TextOnlyPrinter.AppendLine("Gracias por su visita");
            TerminalPrinters.Instance.TextOnlyPrinter.Print();
            //TerminalPrinters.Instance.DriverTextOnlyPrinter.Close();

        }
    }
}
