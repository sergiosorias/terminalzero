using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ZeroBarcode.Pages
{
    /// <summary>
    /// Interaction logic for BarcodePrintView.xaml
    /// </summary>
    public partial class BarcodePrintView : UserControl
    {
        private FixedPage fixedPage;
        private Grid grid;
        private PageContent pageContent;
        private int totalRows = 16;
        private int totalColumns = 6;

        public BarcodePrintView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var dialog = new PrintDialog();
            document.DocumentPaginator.PageSize = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);
            fixedPage = new FixedPage
            {
                Height = document.DocumentPaginator.PageSize.Height,
                Width = document.DocumentPaginator.PageSize.Width
            };
            grid = new Grid();
            grid.Arrange(new Rect(0, 0, fixedPage.Width, fixedPage.Height));
            fixedPage.Children.Add(grid);
            pageContent = new PageContent { Child = fixedPage };
            pageContent.Arrange(new Rect(0, 0, fixedPage.Width, fixedPage.Height));
            fixedPage.Margin = new Thickness(20, 44, 20, 44);

            for (int i = 0; i < totalRows; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < totalColumns; j++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            document.Pages.Add(pageContent);
        }

        private void UpdateBarcodes(TextBlock barcodeText)
        {
            grid.Children.Clear();
            double width = (fixedPage.Width - fixedPage.Margin.Left*2) / totalColumns;
            double height = (fixedPage.Height-fixedPage.Margin.Top*2) / totalRows;
            for (int i = 0; i < totalRows; i++)
            {
                grid.RowDefinitions[i].Height = new GridLength(height);
            }
            for (int j = 0; j < totalColumns; j++)
            {
                grid.ColumnDefinitions[j].Width = new GridLength(width);
            }
            for (int i = 0; i < totalRows; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var tb = new TextBlock
                                    {
                                        //Margin = barcodeText.Margin,
                                        Text = barcodeText.Text,
                                        FontFamily = barcodeText.FontFamily,
                                        FontSize = barcodeText.FontSize
                                    };
                    tb.HorizontalAlignment = HorizontalAlignment.Center;
                    tb.VerticalAlignment = VerticalAlignment.Center;
                    tb.SetValue(Grid.ColumnProperty, j);
                    tb.SetValue(Grid.RowProperty, i);
                    grid.Children.Add(tb);
                    
                }
            }
        }

        private void BarcodeGenerator_PreviewClick(object sender, System.EventArgs e)
        {
            UpdateBarcodes(((Controls.BarcodeGenerator)sender).BarcodeText);
        }

        private void generator_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateBarcodes(((Controls.BarcodeGenerator)sender).BarcodeText);
        }

        
    }


}
