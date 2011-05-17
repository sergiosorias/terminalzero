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
            if(!header.PrintMode.HasValue)
            {
                
            }
        }
    }
}
