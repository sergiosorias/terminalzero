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
using ZeroGUI;

namespace ZeroConfiguration.Pages.Controls
{
    /// <summary>
    /// Interaction logic for UserDetail.xaml
    /// </summary>
    public partial class UserDetail : UserControl, IZeroPage
    {
        public UserDetail()
        {
            InitializeComponent();
            Mode = ZeroCommonClasses.Interfaces.Mode.Update;
        }

        private void grid1_Loaded(object sender, RoutedEventArgs e)
        {
            switch (Mode)
            {
                case Mode.New:
                    lblNameTextBox.Visibility = System.Windows.Visibility.Visible;
                    nameTextBox.Visibility = System.Windows.Visibility.Visible;
                    chbIsApproved.Visibility = System.Windows.Visibility.Collapsed;
                    btnResetPassword.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                default:
                    emailTextBox.DataContext = DataContext;
                    break;
            }
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Esta a punto de cambiar la contraseña por una nueva, ¿esta seguro?","Precaución",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MembershipUser usr = ((MembershipUser)DataContext);
                usr.ChangePassword(usr.GetPassword(), usr.UserName.ToLower());
                MessageBox.Show("La nueva contraseña es: " + usr.UserName.ToLower(), ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
                System.Diagnostics.Trace.WriteLine(string.Format("User {0} Password Changed",usr.ProviderName));
            }
        }

        #region Implementation of IZeroPage

        public Mode Mode { get; set; }
        public bool CanAccept(object parameter)
        {
            bool ret = false;
            switch (Mode)
            {
                case Mode.New:
                    if (string.IsNullOrWhiteSpace(nameTextBox.Text))
                    {
                        MessageBox.Show("Por favos ingrese un nombre!","Error");
                        ret = false;
                    }
                    else
                    {
                        string msg;
                        ret = TryCreateUser(out msg);
                        if(!ret)
                        {
                            MessageBox.Show(msg, "Error");
                        }
                    }
                    break;
                default:
                    MembershipUser usr = ((MembershipUser)DataContext);
                    Membership.UpdateUser(usr);
                    break;
            }
            
            return ret;
        }

        private bool TryCreateUser(out string msg)
        {
            msg = "";
            bool ret = true;
            try
            {
                Membership.CreateUser(nameTextBox.Text, nameTextBox.Text.ToLower(), emailTextBox.Text);
            }
            catch (MembershipCreateUserException e)
            {
                msg = GetErrorMessage(e.StatusCode);
                ret = false;
            }
            catch (Exception e)
            {
                msg = e.Message;
                ret = false;
            }

            return ret;

        }

        private static string GetErrorMessage(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        
        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion
    }
}
