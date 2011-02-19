using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ZeroBarcode.Pages
{
    /// <summary>
    /// Interaction logic for BarcodePrintView.xaml
    /// </summary>
    public partial class BarcodePrintView : UserControl
    {
        public BarcodePrintView()
        {
            InitializeComponent();
        }

        public void Init(TextBlock barcodeText)
        {
            var dialog = new PrintDialog();
            document.PageHeight = dialog.PrintableAreaHeight;
            document.PageWidth = dialog.PrintableAreaWidth;

            document.ColumnWidth = barcodeText.DesiredSize.Width;
            document.IsColumnWidthFlexible = false;
            for (int i = 0; i < 16; i++)
            {
                var tr = new TableRow();
                for (int j = 0; j < 6; j++)
                {
                    var td = new
                        TableCell(
                        new BlockUIContainer
                            (
                            new TextBlock
                                {
                                    Margin = barcodeText.Margin,
                                    Text = barcodeText.Text,
                                    FontFamily = barcodeText.FontFamily,
                                    FontSize = barcodeText.FontSize
                                }
                            )
                        );
                    tr.Cells.Add(td);
                }
                barcodeTableRow.Rows.Add(tr);
            }
            
        }
    }


}
