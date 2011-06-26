using System.Windows;
using System.Windows.Input;
using ZeroGUI;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for CreateSaleView.xaml
    /// </summary>
    public partial class CreateSaleView : NavigationBasePage
    {
        public CreateSaleView()
        {
            InitializeComponent();
            CommandBar.TabIndex = 3;
            CommandBar.Save += CommandBar_New;
        }

        void CommandBar_New(object sender, RoutedEventArgs e)
        {
            ((UIElement)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
        }

        private void BarcodeReceived(object sender, ZeroGUI.Classes.BarCodeEventArgs e)
        {
            Dispatcher.BeginInvoke(new Update(() => { 
                if (sender == lotBarcode) 
                    mainBarcode.SetFocus();
                else
                    lotBarcode.SetFocus();  })
                    );
        }
    }
}
