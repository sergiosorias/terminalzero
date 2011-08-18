using System.Collections;
using System.Windows;
using System.Windows.Controls;
using ZeroPrinters;

namespace ZeroGUI.Reporting
{
    public class ReportBuilder
    {
        public static void Create(string reportHeader, IList source, params ReportColumnInfo[] columns)
        {
            var view = new GridReport {Header = reportHeader, DataSource = source};
            if (columns != null)
                foreach (var info in columns)
                {
                    view.Columns.Add(info);
                }
            PrintReport(view);
        }

        public static void Create(string reportHeader, LazyLoadingListControl list)
        {
            var view = new GridReport { Header = reportHeader, DataSource = list.Items };
            PrintReport(view);
        }

        private static void PrintReport(GridReport view)
        {
            if (ZeroMessageBox.Show(view, ResizeMode.NoResize, MessageBoxButton.OKCancel).GetValueOrDefault())
            {
                PrintDialog dialog = null;
                if ((TerminalPrinters.Instance.GeneralPrinter.IsOnLine && TerminalPrinters.Instance.GeneralPrinter.LoadPrintDialog(out dialog))
                    || (!TerminalPrinters.Instance.IsNeeded(TerminalPrinters.Instance.GeneralPrinter) && dialog.ShowDialog().GetValueOrDefault()))
                {
                    view.SetPageSize(dialog.PrintableAreaHeight, dialog.PrintableAreaWidth);
                    dialog.PrintDocument(view.PaginatorSource.DocumentPaginator, "");
                }
            }
        }

        
    }
}
