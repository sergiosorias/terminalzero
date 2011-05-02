using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroCommonClasses.Interfaces;

namespace ZeroConfiguration.Pages.Controls
{
    /// <summary>
    /// Interaction logic for UserChangePassword.xaml
    /// </summary>
    public partial class UserChangePassword : ZeroGUI.NavigationBasePage
    {
        private MembershipUser _user;
        public UserChangePassword()
        {
            InitializeComponent();
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (!ret || _user.GetPassword() != oldPass.Password)
            {
                ret = false;
                MessageBox.Show(ZeroConfiguration.Properties.Resources.MsgVerifyInsertedInfo, ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
            }
            else
            {
                _user.ChangePassword(_user.GetPassword(), newPass.Password);
                MessageBox.Show(ZeroConfiguration.Properties.Resources.MsgPasswordSuccessfullyModificated, ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
            }
            
            return ret ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _user = DataContext as MembershipUser;
            if (_user == null)
                throw new MissingMemberException("UserChangePassword - No existe un usuario asociado al control!");
        }
    }
}
