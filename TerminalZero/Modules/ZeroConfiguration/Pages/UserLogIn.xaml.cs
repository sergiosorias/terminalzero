using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZeroConfiguration.Pages
{
    /// <summary>
    /// Interaction logic for UserLogIn.xaml
    /// </summary>
    public partial class UserLogIn : UserControl
    {
        public string UserName { get; set; }
        public string UserPass { get; set; }

        public UserLogIn()
        {
            InitializeComponent();
            layoutRoot.DataContext = this;
        }

        private void namePass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UserPass = namePass.Password;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            nameText.Focus();
        }

        private void namePass_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void nameText_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                namePass.Focus();
            }
        }
    }
}
