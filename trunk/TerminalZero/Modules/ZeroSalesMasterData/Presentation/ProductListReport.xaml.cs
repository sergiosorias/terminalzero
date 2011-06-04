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

namespace ZeroMasterData.Reporting
{
    /// <summary>
    /// Interaction logic for ProductListReport.xaml
    /// </summary>
    public partial class ProductListReport : UserControl
    {
        public IEnumerable<Product> ProductList { get; set; }
        public ProductListReport()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var dialog = new PrintDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                document.PageHeight = dialog.PrintableAreaHeight;
                document.PageWidth = dialog.PrintableAreaWidth;
                document.PagePadding = new Thickness(50);
                document.ColumnGap = 0;
                document.ColumnWidth = dialog.PrintableAreaWidth;
                
                IDocumentPaginatorSource dps = document;
                dialog.PrintDocument(dps.DocumentPaginator, "");
            }
        }
    }
}
