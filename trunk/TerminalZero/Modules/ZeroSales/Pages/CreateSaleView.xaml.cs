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
    }
}
