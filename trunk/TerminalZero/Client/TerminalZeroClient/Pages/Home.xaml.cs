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
using ZeroCommonClasses.GlobalObjects;

namespace TerminalZeroClient.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                foreach (ZeroCommonClasses.GlobalObjects.ZeroAction item in App.Instance.CurrentClient.Manager.GetShorcutActions())
                {
                    Button b = new Button();
                    b.Content = item.Name.Substring(item.Name.LastIndexOf('@') + 1).Trim();
                    b.Style = (Style)Resources["homeButtonStyle"];
                    b.DataContext = item;
                    b.Click += new RoutedEventHandler(btn_Click);
                    ButtonsContent.Children.Add(b);
                }

            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = (FrameworkElement)sender;
            ZeroAction buttonAction = item.DataContext as ZeroAction;

            if (buttonAction != null)
            {
                try
                {
                    string msg;

                    if (!App.Instance.CurrentClient.Manager.Navigate(out msg, buttonAction) && !string.IsNullOrWhiteSpace(msg))
                    {
                        System.Windows.Forms.MessageBox.Show("No se ha podido realizar la acción deseada\n\nProblemas:\n" + msg, "Información", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString(), "Error inesperado",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }

            }
        }
    }
}
