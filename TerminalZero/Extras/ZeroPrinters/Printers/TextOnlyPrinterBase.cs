using System.Collections.Generic;
using System.Text;

namespace ZeroPrinters.Printers
{
    public abstract class TextOnlyPrinterBase : General
    {
        private const string kColumns = "Columns";
        protected string newLineSeparator = "\n\r\n";

        protected StringBuilder Data;

        protected readonly List<int> columnsWidth = new List<int>();

        public int LineCount { get; private set; }

        public int MaxColumns { get; private set; }

        protected TextOnlyPrinterBase(PrinterInfo info)
            : base(info)
        {
            if (info.InitializeParameters != null)
            {
                MaxColumns = info.InitializeParameters.ContainsKey(kColumns) ? int.Parse(info.InitializeParameters[kColumns]) : 44;
            }
        }

        protected void InitializeQueue()
        {
            Data.Clear();
            LineCount = 0;
            AppendLine("Cabana del Rey");
            AppendLine();
        }

        public void AppendLine(string line, char fillChar = ' ')
        {
            LineCount++;
            Data.Append(line.PadRight(MaxColumns, fillChar) + newLineSeparator);
        }

        public void AppendColumnsLine(params string[] columns)
        {
            if(columnsWidth.Count == 0 || columnsWidth.Count != columns.Length)
            {
                columnsWidth.Clear();
                foreach (var column in columns)
                {
                    columnsWidth.Add(column.Length+1);
                }
            }
            string final = string.Empty;
            for (int i = 0; i < columns.Length; i++)
            {
                final += columns[i].PadRight(columnsWidth[i]);
            }

            AppendLine(final);
        }

        public void AppendLine()
        {
            AppendLine(string.Empty);
        }

        public new abstract void Print();
        
        public override void CancelPrint()
        {
            InitializeQueue();
        }
    }
}