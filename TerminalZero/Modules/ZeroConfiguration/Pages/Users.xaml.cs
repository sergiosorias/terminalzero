using System.Web.Security;
using System.Windows.Controls;
using System.Windows;

namespace ZeroConfiguration.Pages
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : UserControl
    {
        private System.Web.Security.MembershipUserCollection _userCol;
        public Users()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                _userCol = System.Web.Security.Membership.GetAllUsers();

                foreach (MembershipUser user in _userCol)
                {
                    users.Items.Add(user);
                }
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            Controls.UserDetail ud = new Controls.UserDetail();
            ud.DataContext = Membership.GetUser(((Button) sender).DataContext);
            ZeroGUI.ZeroMessageBox.Show(ud, "Editar Usuario", SizeToContent.WidthAndHeight);
        }
    }
}
