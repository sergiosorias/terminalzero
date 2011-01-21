using System.Web.Security;
using System.Windows.Controls;
using System.Windows;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroConfiguration.Pages
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : UserControl
    {
        private System.Web.Security.MembershipUserCollection _userCol;
        private readonly ITerminal _terminal;
        public Users(ITerminal terminal)
        {
            InitializeComponent();
            _terminal = terminal;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ZeroAction action;
            if (_terminal.Manager.ExistsAction("Configuración@Usuarios@Cambiar contraseña", out action))
            {
                btnChangePassword.Command = action;
            }
            else
            {
                btnChangePassword.IsEnabled = false;
            }

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            users.Items.Clear();
            _userCol = System.Web.Security.Membership.GetAllUsers();

            foreach (MembershipUser user in _userCol)
            {
                users.Items.Add(user);
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            var ud = new Controls.UserDetail();
            MembershipUser usr = null;
            foreach (MembershipUser user in _userCol)
            {
                if (user.ProviderUserKey.Equals(((Button)sender).DataContext))
                {
                    usr = user;
                    break;
                }
            }
            
            ud.DataContext = usr;
            if(ZeroGUI.ZeroMessageBox.Show(ud, ZeroConfiguration.Properties.Resources.EditUser, SizeToContent.WidthAndHeight).GetValueOrDefault())
            {
                users.UpdateLayout();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var ud = new Controls.UserDetail();
            ud.Mode = Mode.New;
            if (ZeroGUI.ZeroMessageBox.Show(ud, ZeroConfiguration.Properties.Resources.NewUser, SizeToContent.WidthAndHeight).GetValueOrDefault())
            {
                LoadUsers();
            }
        }
    }
}
