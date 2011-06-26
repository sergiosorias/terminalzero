using ZeroGUI;
using ZeroSales.Presentation;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for SaleReportView.xaml
    /// </summary>
    public partial class SaleReportView : NavigationBasePage
    {
        public SaleReportView()
        {
            InitializeComponent();
            Loaded += (o,e) => CommandBar.AppendButton("Actualizar", ((SaleReportViewModel) ViewModel).UpdateCommand);
        }
    }
}
