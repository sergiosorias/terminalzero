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
using ZeroConfiguration.Presentantion;
using ZeroGUI;

namespace ZeroConfiguration.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : NavigationBasePage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void NavigationBasePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode)
            {
                DataContext = new MainViewModel();
            }
        }

        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteSelectedAction();
            }
        }

        private void algo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ExecuteSelectedAction();
        }

        private void ExecuteSelectedAction()
        {
            ((MainViewModel) DataContext).SelectedAction.Action.TryExecute();
        }

    }
}
