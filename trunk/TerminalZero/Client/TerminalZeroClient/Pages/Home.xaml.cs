using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.GlobalObjects;

namespace TerminalZeroClient.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (ZeroCommonClasses.GlobalObjects.ZeroAction item in App.Instance.Manager.GetShorcutActions())
                {
                    Button b = new Button();
                    b.Content = item.Alias;
                    b.Style = (Style)Resources["homeButtonStyle"];
                    b.DataContext = item;
                    b.Click += new RoutedEventHandler(btn_Click);
                    ButtonsContent.Children.Add(b);
                }

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = (FrameworkElement)sender;
            ZeroAction buttonAction = item.DataContext as ZeroAction;

            if (buttonAction != null)
            {
                App.Instance.Manager.ExecuteAction(buttonAction);
            }
        }
    }
}
