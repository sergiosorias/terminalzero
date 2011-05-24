using System.Collections;
using System.Windows;
using System.Windows.Controls;

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
            if(ZeroMessageBox.Show(view, ResizeMode.NoResize, MessageBoxButton.OKCancel).GetValueOrDefault())
            {
                var dialog = new PrintDialog();
                if (dialog.ShowDialog().GetValueOrDefault())
                {
                    view.SetPageSize(dialog.PrintableAreaHeight,dialog.PrintableAreaWidth);
                    dialog.PrintDocument(view.PaginatorSource.DocumentPaginator, "");
                }
            }
        }

        public static void Create(string reportHeader, LazyLoadingListControl list)
        {
            var view = new GridReport { Header = reportHeader, DataSource = list.Items };
            
            if (ZeroMessageBox.Show(view, ResizeMode.NoResize, MessageBoxButton.OKCancel).GetValueOrDefault())
            {
                var dialog = new PrintDialog();
                if (dialog.ShowDialog().GetValueOrDefault())
                {
                    view.SetPageSize(dialog.PrintableAreaHeight, dialog.PrintableAreaWidth);
                    dialog.PrintDocument(view.PaginatorSource.DocumentPaginator, "");
                }
            }
        }
    }
}
