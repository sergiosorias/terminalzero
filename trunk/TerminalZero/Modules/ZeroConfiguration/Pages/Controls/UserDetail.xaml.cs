using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

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
        }

        private void btnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if(ZeroMessageBox.Show("Esta a punto de cambiar la contraseña por una nueva, ¿esta seguro?","Precaución",MessageBoxButton.YesNo).GetValueOrDefault())
            {
                var usr = ((User)DataContext);
                usr.ResetPassword();
                ZeroMessageBox.Show("La nueva contraseña es: " + usr.Password, ZeroConfiguration.Properties.Resources.Information, MessageBoxButton.OK);
                Trace.WriteLine(string.Format("User {0} Password Changed",usr.UserName));
            }
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
            var a = new Binding("UserName");

            switch (newMode)
            {
                case ControlMode.New:
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
