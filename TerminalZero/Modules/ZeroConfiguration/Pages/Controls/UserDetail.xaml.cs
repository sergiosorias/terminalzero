using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Interfaces;

namespace ZeroConfiguration.Pages.Controls
{
    /// <summary>
    /// Interaction logic for UserDetail.xaml
    /// </summary>
    public partial class UserDetail : ZeroGUI.NavigationBasePage
    {
        public UserDetail()
        {
            InitializeComponent();
            ControlMode = ControlMode.Update;
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Esta a punto de cambiar la contraseña por una nueva, ¿esta seguro?","Precaución",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var usr = ((User)DataContext);
                usr.ChangePassword(usr.GetPassword(), usr.UserName.ToLower());
                MessageBox.Show("La nueva contraseña es: " + usr.UserName.ToLower(), ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
                Trace.WriteLine(string.Format("User {0} Password Changed",usr.UserName));
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
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
                        ret = User.TryCreateUser(nameTextBox.Text, nameTextBox.Text.ToLower(), emailTextBox.Text,out msg);
                        if(!ret)
                        {
                            MessageBox.Show(msg, "Error");
                        }
                    }
                    break;
                default:
                    var usr = ((User)DataContext);
                    User.UpdateUser(usr);
                    ret = true;
                    break;
            }
            
            return ret;
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
            var a = new Binding("UserName");

            switch (newMode)
            {
                case ControlMode.New:
                    lblNameTextBox.Visibility = Visibility.Visible;
                    nameTextBox.Visibility = Visibility.Visible;
                    chbIsApproved.Visibility = Visibility.Collapsed;
                    btnResetPassword.Visibility = Visibility.Collapsed;
                    a.Mode = BindingMode.TwoWay;
                    break;
                default:
                    emailTextBox.DataContext = DataContext;
                    a.Mode = BindingMode.OneWay;
                    break;
            }
        }

        

        
        
       
    }
}
