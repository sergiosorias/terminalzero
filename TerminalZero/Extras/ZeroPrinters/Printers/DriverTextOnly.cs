using System.IO;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ZeroPrinters.Printers
{
    public class DriverTextOnly : TextOnlyPrinterBase
    {
        private const string kFontFamilyName = "Courier New";
        private const int kFontSize = 9;

        private static FontFamily font = new FontFamily(kFontFamilyName);

        public DriverTextOnly(PrinterInfo info)
            :base(info)
        {
            InitializeQueue();
        }

        public override bool IsOnLine
        {
            get
            {
                PrintDialog dialog;
                return LoadPrintDialog(out dialog);
            }
            protected set
            {

            }
        }

        public override void Print()
        {
            PrintFlowDocument();
        }

        private void PrintFixedDocument()
        {
            PrintDialog dialog;
            if (LoadPrintDialog(out dialog))
            {
                var document = new FixedDocument();
                var page = new FixedPage();
                var t = new TextBlock
                            {
                                FontFamily = font,
                                FontSize = kFontSize,
                                Text = Data.ToString()
                            };
                page.Children.Add(t);
                t.Measure(new Size(document.DocumentPaginator.PageSize.Width, document.DocumentPaginator.PageSize.Width));
                page.Height = t.DesiredSize.Height;
                page.Width = t.DesiredSize.Width;
                var p = new PageContent { Child = page };
                document.Pages.Add(p);

                Print(document, dialog);
            }
        }

        private void PrintFlowDocument()
        {
            PrintDialog dialog;
            if (LoadPrintDialog(out dialog))
            {
                var fdoc = new FlowDocument();
                var parag = new Paragraph(new Run(Data.ToString()))
                {
                    FontSize = kFontSize, 
                    FontFamily = font,
                                    
                };
                fdoc.Blocks.Add(parag);

                Print(fdoc, dialog);
            }
        }

        private void Print(IDocumentPaginatorSource document, PrintDialog dialog)
        {
            document.DocumentPaginator.PageSize = new Size(MaxColumns * 7, LineCount * 15);
            dialog.PrintTicket.PageMediaSize = new PageMediaSize(MaxColumns*7, LineCount*15+2);
            dialog.PrintDocument(document.DocumentPaginator, "");
        }
    }
}