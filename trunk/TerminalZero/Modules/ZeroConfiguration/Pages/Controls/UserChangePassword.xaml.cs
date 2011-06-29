using System;
using System.Windows;
using ZeroBusiness.Entities.Configuration;
using ZeroGUI;

namespace ZeroConfiguration.Pages.Controls
{
    /// <summary>
    /// Interaction logic for UserChangePassword.xaml
    /// </summary>
    public partial class UserChangePassword : NavigationBasePage
    {
        private User _user;
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
                ZeroMessageBox.Show(ZeroConfiguration.Properties.Resources.MsgVerifyInsertedInfo, ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
            }
            else
            {
                _user.ChangePassword(_user.GetPassword(), newPass.Password);
                ZeroMessageBox.Show(ZeroConfiguration.Properties.Resources.MsgPasswordSuccessfullyModificated, ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
            }
            
            return ret ;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _user = DataContext as User;
            if (_user == null)
                throw new MissingMemberException("UserChangePassword - No existe un usuario asociado al control!");
        }
    }
}
