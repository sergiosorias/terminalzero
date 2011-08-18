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
            if (header.PrintMode == 0)
            {
                //TerminalPrinters.Instance.TextOnlyPrinter.Open();
                TerminalPrinters.Instance.TextOnlyPrinter.Clear();
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("Fecha: {0}   Hora: {1}", DateTime.Now.Date.ToString("dd/MM/yy"), DateTime.Now.ToString("hh:mm:ss")));
                TerminalPrinters.Instance.TextOnlyPrinter.AppendColumnsLine("Descripcion    ", "Cant.   ", "P.Unit.  ", "Precio");
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine("=", '=');
                foreach (var saleItem in header.SaleItems)
                {
                    TerminalPrinters.Instance.TextOnlyPrinter.AppendLine(saleItem.Product.Name);
                    TerminalPrinters.Instance.TextOnlyPrinter.AppendColumnsLine("", saleItem.Quantity.ToString("0.00"), saleItem.Product.Price1.Value.ToString("0.00"), saleItem.PriceValue.ToString("0.00"));
                }
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine("=", '=');
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine();
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine(string.Format("Total: ${0}", header.PriceSumValue));
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine();
                TerminalPrinters.Instance.TextOnlyPrinter.AppendLine("Gracias por su compra");
                TerminalPrinters.Instance.TextOnlyPrinter.Print();
                //TerminalPrinters.Instance.TextOnlyPrinter.Close();
            }
            else
            {
                if (TerminalPrinters.Instance.LegalPrinter != null && TerminalPrinters.Instance.LegalPrinter.IsOnLine)
                {
                    TerminalPrinters.Instance.LegalPrinter.Clear();
                    TerminalPrinters.Instance.LegalPrinter.OpenInvoice(null);
                    if (!TerminalPrinters.Instance.LegalPrinter.HasError)
                    {
                        foreach (var saleItem in header.SaleItems)
                        {
                            if (!TerminalPrinters.Instance.LegalPrinter.HasError)
                            {
                                TerminalPrinters.Instance.LegalPrinter.PrintItem(saleItem.Product.Name, 1, saleItem.PriceValue, saleItem.Product.Tax.Value, 0);
                            }
                        }
                        if (!TerminalPrinters.Instance.LegalPrinter.HasError)
                        {
                            TerminalPrinters.Instance.LegalPrinter.Print();
                        }
                    }
                }
            }
        }

        public static void PrintZReport(DateTime selectedDate)
        {
            if(selectedDate!=null)
            {
                if (TerminalPrinters.Instance.LegalPrinter != null && TerminalPrinters.Instance.LegalPrinter.IsOnLine) 
                    TerminalPrinters.Instance.LegalPrinter.PrintZReport(selectedDate);
            }
            else
            {
                if (TerminalPrinters.Instance.LegalPrinter != null && TerminalPrinters.Instance.LegalPrinter.IsOnLine) 
                    TerminalPrinters.Instance.LegalPrinter.PrintZReport();
            }
        }
    }
}
