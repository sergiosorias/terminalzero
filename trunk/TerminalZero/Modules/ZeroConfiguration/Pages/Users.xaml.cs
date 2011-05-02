using System;
using System.ComponentModel;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroConfiguration.Pages.Controls;
using ZeroGUI;

namespace ZeroConfiguration.Pages
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : UserControl
    {
        private MembershipUserCollection _userCol;
        public Users()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ZeroAction action;
            if (ZeroCommonClasses.Terminal.Instance.Manager.ExistsAction(ZeroBusiness.Actions.OpenUserPasswordChangeMessage, out action))
            {
                btnChangePassword.Command = action;
            }
            else
            {
                btnChangePassword.IsEnabled = false;
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                LoadUsers();
            }
        }

        private void LoadUsers()
        {
            users.Items.Clear();
            _userCol = Membership.GetAllUsers();

            foreach (MembershipUser user in _userCol)
            {
                users.Items.Add(user);
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ud = new UserDetail();
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
                if (ZeroMessageBox.Show(ud, ZeroConfiguration.Properties.Resources.EditUser, SizeToContent.WidthAndHeight).GetValueOrDefault())
                {
                    users.UpdateLayout();
                }
            }
            catch (Exception ex)
            {
                ZeroMessageBox.Show(ex, "Error", ResizeMode.NoResize, MessageBoxButton.OK);
            }
            
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var ud = new UserDetail();
            ud.ControlMode = ControlMode.New;
            if (ZeroMessageBox.Show(ud, ZeroConfiguration.Properties.Resources.NewUser, SizeToContent.WidthAndHeight).GetValueOrDefault())
            {
                LoadUsers();
            }
        }
    }
}
