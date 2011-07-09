using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ZeroPrinters
{
    public class SystemPrinters
    {
        #region Singleton
        private SystemPrinters()
        {
        }

        private static SystemPrinters _Instance;
        public static SystemPrinters Instance
        {
            get { return _Instance ?? (_Instance = new SystemPrinters()); }
        }
        #endregion

        public void Load(List<PrinterInfo> printersConfig)
        {
            foreach (PrinterInfo printerInfo in printersConfig)
            {
                switch (printerInfo.Type)
                {
                    case (int)PrinterType.General:
                        GeneralPrinter = new GeneralPrinter(printerInfo.Name);
                        break;
                    case (int)PrinterType.Legal:
                        break;
                    case (int)PrinterType.TextOnly:
                        TextOnlyPrinter = new TextOnlyPrinter(printerInfo.Name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            GeneralPrinter = GeneralPrinter ?? (new GeneralPrinter("GeneralPrinter Unknown"));
            TextOnlyPrinter = TextOnlyPrinter ?? (new TextOnlyPrinter("TextOnlyPrinter Unknown"));
        }

        public GeneralPrinter GeneralPrinter { get; private set; }
        public LegalPrinter LegalPrinter { get; private set; }
        public TextOnlyPrinter TextOnlyPrinter { get; private set; }
    }

    public struct PrinterInfo
    {
        public string Name;
        public int Type;
        public Dictionary<string, string> InitializeParameters;
    }

    public abstract class SystemPrinter
    {
        public SystemPrinter(string name)
        {
            Name = name;
            Parameters = new Dictionary<string, string>();
        }

        public string Name { get; internal set; }
        public abstract bool Exists { get; }
        public bool IsExistanceMandatory { get; protected set; }
        public Dictionary<string,string> Parameters { get; private set; }
    }

    public class LegalPrinter : SystemPrinter
    {
        public LegalPrinter(string name)
            :base(name)
        {
            
        }

        #region Overrides of SystemPrinter

        public override bool Exists
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class TextOnlyPrinter : GeneralPrinter
    {
        private StringBuilder printQueue;
        public int MaxColumns { get; set; }
        private readonly List<int> columnsWidth = new List<int>();
        public TextOnlyPrinter(string name)
            :base(name)
        {
            printQueue = new StringBuilder();
            MaxColumns = 40;
            InitializeQueue();
        }

        private void InitializeQueue()
        {
            printQueue.Clear();
            AppendLine("Cabaña del Rey");
            AppendLine();
        }

        #region Overrides of SystemPrinter

        public override bool Exists
        {
            get
            {
                PrintDialog dialog;
                return LoadPrintDialog(out dialog);

            }
        }
        
        public void AppendLine(string line, char fillChar = ' ')
        {
            printQueue.AppendLine(line.PadRight(MaxColumns, fillChar));
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
            AppendLine("");
        }

        public void Print()
        {
            PrintDialog dialog;
            if(LoadPrintDialog(out dialog))
            {
                var document = new FixedDocument();
                var page = new FixedPage();
                TextBlock t = new TextBlock();
                t.FontFamily = new System.Windows.Media.FontFamily("Global Monospace");
                t.Text = printQueue.ToString();
                page.Children.Add(t);
                t.Measure(new Size(document.DocumentPaginator.PageSize.Width, document.DocumentPaginator.PageSize.Width));
                page.Height = t.DesiredSize.Height+30;
                page.Width = t.DesiredSize.Width;
                var p = new PageContent {Child = page};
                document.Pages.Add(p);
                dialog.PrintDocument(document.DocumentPaginator,"");
                CancelPrint();
            }
        }

        public void CancelPrint()
        {
            InitializeQueue();
        }

        #endregion
    }

    public class GeneralPrinter : SystemPrinter
    {
        public GeneralPrinter(string name)
            :base(name)
        {
            IsExistanceMandatory = false;
        }

        public bool LoadPrintDialog(out PrintDialog dialog)
        {
            dialog = new PrintDialog();
            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues();
            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {
                if (printer.Name == Name)
                {
                    dialog.PrintQueue = printer;
                    return true;
                }
            }
            return false;
        }

        #region Overrides of SystemPrinter

        public override bool Exists
        {
            get { return true; }
        }

        #endregion
    }

    public enum PrinterType
    {
        General = 1,
        Legal = 2,
        TextOnly = 4
    }
}
