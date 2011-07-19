using System.Windows;
using System.Windows.Input;
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

        private void NavigationBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            if(ViewModel!=null)
                CommandBar.AppendButton(Properties.Resources.Change, ((SalePaymentViewModel)ViewModel).CustomerSelectionCommand);

            addPaymentInstrument.IsVisibleChanged += addPaymentInstrument_IsVisibleChanged;
        }

        void addPaymentInstrument_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((UIElement)sender).IsFocused)
                ((UIElement) sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
