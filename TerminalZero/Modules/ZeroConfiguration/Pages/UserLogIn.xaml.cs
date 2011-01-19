using System.Windows;
using System.Windows.Controls;

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
    }
}
