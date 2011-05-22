using System;
using System.Collections.Generic;
using System.Linq;
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
using ZeroBusiness.Entities.Data;
using ZeroGUI;

namespace ZeroMasterData.Reporting
{
    /// <summary>
    /// Interaction logic for ProductListReport.xaml
    /// </summary>
    public partial class ProductListReport : NavigationBasePage
    {
        public List<Product> ProductList { get; set; }
        public ProductListReport()
        {
            InitializeComponent();
            base.CommandBar.Print += new RoutedEventHandler(CommandBar_Print);
        }

        void CommandBar_Print(object sender, RoutedEventArgs e)
        {
            var dialog = new PrintDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                document.MaxPageHeight = dialog.PrintableAreaHeight;
                document.MaxPageWidth = dialog.PrintableAreaWidth;
                IDocumentPaginatorSource dps = document;
                dialog.PrintDocument(dps.DocumentPaginator, "");
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var dialog = new PrintDialog();
            document.PageHeight = dialog.PrintableAreaHeight;
            document.PageWidth = dialog.PrintableAreaWidth;
            int i = 1;
            foreach (Product product in ProductList)
            {
                TableRow tr = new TableRow();
                if (i++ % 2 == 0)
                    tr.Background = (SolidColorBrush) Application.Current.Resources["BlueBrushKey"];
                
                var cell1 = new TableCell(new Paragraph(new Run(product.MasterCode)));
                tr.Cells.Add(cell1);
                cell1.Style = (Style)Resources["itemCell"];
                var cell2 = new TableCell(new Paragraph(new Run(product.Name)));
                cell2.Style = (Style)Resources["itemCell"];
                tr.Cells.Add(cell2);
                var cell3 = new TableCell(new Paragraph(new Run(string.Format("$ {0}", product.Price1.Value))));
                cell3.TextAlignment = TextAlignment.Center;
                cell3.Style = (Style)Resources["itemCell"];
                tr.Cells.Add(cell3);
                table1.RowGroups[1].Rows.Add(tr);
            }
        }



    }
}
