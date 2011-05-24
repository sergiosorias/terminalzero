using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroGUI.Reporting
{
    /// <summary>
    /// Interaction logic for GridReport.xaml
    /// </summary>
    public partial class GridReport : NavigationBasePage
    {
        public List<ReportColumnInfo> Columns { get; private set; }
        public IList DataSource { get; set; }
        public IDocumentPaginatorSource PaginatorSource { get { return document; } }
        private PropertyInfo[] itemProperties = null;

        internal GridReport()
        {
            InitializeComponent();
            Columns = new List<ReportColumnInfo>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var dialog = new PrintDialog();
            document.MaxPageHeight = dialog.PrintableAreaHeight;
            document.MaxPageWidth = dialog.PrintableAreaWidth;
            document.PageHeight = dialog.PrintableAreaHeight;
            document.PageWidth = dialog.PrintableAreaWidth;
            LoadItems();
            LoadHeader();
        }

        private void LoadHeader()
        {
            TableRow tr = new TableRow();
            tr.Style = (Style)Resources["headerRow"];
            if (Columns.Count > 0)
            {
                foreach (var columnInfo in Columns)
                {
                    table1.Columns.Add(columnInfo);
                    var cell = new TableCell(new Paragraph(new Run(columnInfo.Name)));
                    cell.Style = (Style) Resources["headerCell"];
                    tr.Cells.Add(cell);

                }
            }
            else
            {
                foreach (var columnInfo in itemProperties)
                {
                    table1.Columns.Add(new ReportColumnInfo(columnInfo.Name,GridLength.Auto));
                    var cell = new TableCell(new Paragraph(new Run(columnInfo.Name)));
                    cell.Style = (Style)Resources["headerCell"];
                    tr.Cells.Add(cell);

                }
            }
            table1.RowGroups[0].Rows.Add(tr);
        }

        private void LoadItems()
        {
            int i = 1;
            
            foreach (var item in DataSource)
            {
                TableRow tr = new TableRow();
                if (i++ % 2 == 0)
                    tr.Background = (SolidColorBrush) Application.Current.Resources["BlueBrushKey"];
                if (itemProperties == null)
                    itemProperties = item.GetType().GetProperties().Where(prop => prop.CanRead).ToArray();
                object value;
                foreach (PropertyInfo t in itemProperties)
                {
                    value = t.GetValue(item, null);
                    var cell = new TableCell(new Paragraph(new Run(value==null?"":value.ToString())));
                    cell.Style = (Style)Resources["itemCell"];
                    tr.Cells.Add(cell);
                }

                table1.RowGroups[1].Rows.Add(tr);
            }
        }

        public void SetPageSize(double height, double width)
        {
            document.MaxPageHeight = height;
            document.MaxPageWidth = width;
            document.PageHeight = height;
            document.PageWidth = width;
        }
    
    }
}
