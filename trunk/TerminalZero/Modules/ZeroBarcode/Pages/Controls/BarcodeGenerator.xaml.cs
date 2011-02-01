using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using ZeroGUI;

namespace ZeroBarcode.Pages.Controls
{
    /// <summary>
    /// Interaction logic for BarcodeGenerator.xaml
    /// </summary>
    public partial class BarcodeGenerator : UserControl
    {
        public BarcodeGenerator()
        {
            InitializeComponent();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (date.SelectedDate != null)
            {
                string chain = string.Format("{0:0000000}{1:00}{2:00}0", date.SelectedDate.Value.Year, date.SelectedDate.Value.Month, date.SelectedDate.Value.Day);
                BarcodeText.Text = EANBarcode.Instance.EAN13(chain) + EANBarcode.Instance.AddOn(chain);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            date.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new PrintDialog();

                var document = new FixedDocument();
                document.DocumentPaginator.PageSize = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight);

                var page1 = new FixedPage();
                page1.Width = document.DocumentPaginator.PageSize.Width;
                page1.Height = document.DocumentPaginator.PageSize.Height;
                var G = new Grid();
                G.Arrange(new Rect(0, 0, page1.Width, page1.Height));
                int I = 0;
                double widthCount = 0, heightCount = 0;
                while (page1.Width > (widthCount + BarcodeText.DesiredSize.Width))
                {
                    G.ColumnDefinitions.Add(new ColumnDefinition());
                    var WP = new StackPanel();
                    widthCount += BarcodeText.DesiredSize.Width;
                    WP.Arrange(new Rect(0, 0, BarcodeText.DesiredSize.Width, page1.Height));
                    WP.SetValue(Grid.ColumnProperty, I++);
                    while (page1.Height > (heightCount + BarcodeText.DesiredSize.Height))
                    {
                        heightCount += BarcodeText.DesiredSize.Height;
                        var tb = new TextBlock
                        {
                            Margin = BarcodeText.Margin,
                            Text = BarcodeText.Text,
                            FontFamily = BarcodeText.FontFamily,
                            FontSize = BarcodeText.FontSize
                        };
                        WP.Children.Add(tb);
                    }
                    heightCount = 0;
                    G.Children.Add(WP);
                }

                page1.Children.Add(G);

                var page1Content = new PageContent();
                ((IAddChild)page1Content).AddChild(page1);
                document.Pages.Add(page1Content);

                var DV = new DocumentViewer();
                DV.HorizontalAlignment = HorizontalAlignment.Stretch;
                DV.VerticalAlignment = VerticalAlignment.Stretch;
                DV.Document = document;
                DV.Height = 415;
                DV.Width = 490;
                ZeroMessageBox.Show(DV, "Imprimir", SizeToContent.WidthAndHeight, ResizeMode.CanResize, MessageBoxButton.OK);

                //BarcodeText.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
                //BarcodeText.Arrange(new Rect(new Point(0, 0), BarcodeText.DesiredSize));
                //dialog.PrintVisual(BarcodeText, "Auxiliar Barcodes");
            }
            catch (PrintDialogException ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
            catch (Exception ex2)
            {

            }

        }
    }
}
