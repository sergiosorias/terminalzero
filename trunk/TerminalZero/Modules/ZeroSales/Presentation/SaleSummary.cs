using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ZeroSales.Presentation
{
    public class ObjectCollection : Collection<object>
    {

    }

    public class TerminalReport
    {
        public string TerminalName { get; set; }
        public int TerminalCode { get; set; }
        public IEnumerable<object> Items { get; set; }
    }

    public class SaleSummary
    {
        public DateTime Date { get; set; }
        public int SalesCount { get; set; }
        public double ValueSum { get; set; }
    }

}
