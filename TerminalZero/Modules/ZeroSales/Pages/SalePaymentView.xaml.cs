using System.Windows;
using ZeroGUI;
using ZeroSales.Presentation;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for SalePaymentView.xaml
    /// </summary>
    public partial class SalePaymentView : NavigationBasePage
    {
        public SalePaymentView()
        {
            InitializeComponent();
        }

        private void NavigationBasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if(ViewModel!=null)
                CommandBar.AppendButton(Properties.Resources.Change, ((SalePaymentViewModel)ViewModel).CustomerSelectionCommand);

            addPaymentInstrument.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(addPaymentInstrument_IsVisibleChanged);
        }

        void addPaymentInstrument_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (((UIElement)sender).IsFocused)
                ((UIElement) sender).MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
        }
    }
}
