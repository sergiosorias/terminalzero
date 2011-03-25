using System;
using System.Diagnostics;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;

namespace ZeroConfiguration.Pages.Controls
{
    /// <summary>
    /// Interaction logic for UserDetail.xaml
    /// </summary>
    public partial class UserDetail : ZeroGUI.ZeroBasePage
    {
        public UserDetail()
        {
            InitializeComponent();
            ControlMode = ControlMode.Update;
        }

        private void grid1_Loaded(object sender, RoutedEventArgs e)
        {
            var a = new System.Windows.Data.Binding("UserName");
            
            switch (ControlMode)
            {
                case ControlMode.New:
                    lblNameTextBox.Visibility = Visibility.Visible;
                    nameTextBox.Visibility = Visibility.Visible;
                    chbIsApproved.Visibility = Visibility.Collapsed;
                    btnResetPassword.Visibility = Visibility.Collapsed;
                    a.Mode = System.Windows.Data.BindingMode.TwoWay;
                    break;
                default:
                    emailTextBox.DataContext = DataContext;
                    a.Mode = System.Windows.Data.BindingMode.OneWay;
                    break;
            }
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Esta a punto de cambiar la contraseña por una nueva, ¿esta seguro?","Precaución",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var usr = ((MembershipUser)DataContext);
                usr.ChangePassword(usr.GetPassword(), usr.UserName.ToLower());
                MessageBox.Show("La nueva contraseña es: " + usr.UserName.ToLower(), ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
                Trace.WriteLine(string.Format("User {0} Password Changed",usr.ProviderName));
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = false;
            switch (ControlMode)
            {
                case ControlMode.New:
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
                    var usr = ((MembershipUser)DataContext);
                    Membership.UpdateUser(usr);
                    ret = true;
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
        
       
    }
}
