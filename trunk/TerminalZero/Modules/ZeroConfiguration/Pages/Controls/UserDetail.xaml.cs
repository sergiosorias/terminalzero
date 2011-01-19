using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroConfiguration.Pages.Controls
{
    /// <summary>
    /// Interaction logic for UserDetail.xaml
    /// </summary>
    public partial class UserDetail : UserControl
    {
        public UserDetail()
        {
            InitializeComponent();
        }

        private void grid1_Loaded(object sender, RoutedEventArgs e)
        {
            nameTextBox.DataContext = DataContext;
            emailTextBox.DataContext = DataContext;
        }
    }
}
