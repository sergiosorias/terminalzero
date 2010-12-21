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
using TerminalZeroClient.Business;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Helpers.MainStatusNotifier Notifier;

        public MainWindow()
        {
            InitializeComponent();
            Notifier = new Helpers.MainStatusNotifier(Dispatcher, statusMsg, statusBar, btnGetMoreStatusInfo);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.Instance.CurrentClient.Manager.ConfigurationRequired += new EventHandler(Manager_ConfigurationRequired);
            App.Instance.CurrentClient.Session.Notifier = Notifier;
            App.Instance.CurrentClient.Session.ModuleList.ForEach(m=>m.Notifing+=new EventHandler<ModuleNotificationEventArgs>(m_Notifing));
            LoadConfigs();
        }

        void Manager_ConfigurationRequired(object sender, EventArgs e)
        {
            LoadConfigs();
        }

        private void LoadConfigs()
        {
            mainMenu.Items.Clear();
            MenuItem item = new MenuItem();
            item.Style = (Style)Resources["masterMenuItem"];
            item.Header = "Inicio";
            item.Click += MenuItem_Click;
            mainMenu.Items.Add(item);
            InternalBuildMenu(App.Instance.CurrentClient.BuildMenu(), mainMenu.Items);
        }

        private void m_Notifing(object sender, ModuleNotificationEventArgs e)
        {
            if (e.SomethingToShow)
            {
                if (PrimaryWindow.Content is IZeroPage)
                {
                    ((IZeroPage)PrimaryWindow.Content).CanAccept();
                }
                PrimaryWindow.Content = e.ControlToShow;
            }
        }

        private void InternalBuildMenu(TerminalZeroClient.Extras.ZeroMenu menu, ItemCollection items)
        {
            foreach (var item in menu)
            {
                MenuItem menuitem = new MenuItem();
                menuitem.Header = item.Key;
                if (item.Value.Count > 0)
                    InternalBuildMenu(item.Value, menuitem.Items);
                else
                {
                    menuitem.DataContext = item.Value.MenuAction;
                    if(!item.Value.MenuAction.AlwaysVisible)
                        menuitem.SetBinding(MenuItem.IsEnabledProperty, "Enabled");

                    if (item.Value.MenuAction.RuleToSatisfy != null && !item.Value.MenuAction.Enabled)
                    {
                        ToolTipService.SetToolTip(menuitem,item.Value.MenuAction.RuleToSatisfy.Result);
                        ToolTipService.SetShowOnDisabled(menuitem, true);    
                    }

                    menuitem.Click += new RoutedEventHandler(menuitemitem_Click);
                }
                                
                items.Add(menuitem);
            }
        }

        protected void menuitemitem_Click(object sender, RoutedEventArgs e)
        {
            mainBar.IsEnabled = false;
            MenuItem item = (MenuItem)sender;
            ZeroAction buttonAction = item.DataContext as ZeroAction;
            
            if(buttonAction!=null)
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
                    System.Windows.Forms.MessageBox.Show(ex.ToString(),"Error inesperado", 
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
                
            }
            
            mainBar.IsEnabled = true;
        }

        private void btnGetMoreStatusInfo_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = new TextBox();
            tb.Text = Notifier.Messages.ToString();
            Notifier.Messages.Clear();
            btnGetMoreStatusInfo.Visibility = System.Windows.Visibility.Hidden;
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tb.TextWrapping = TextWrapping.Wrap;
            ZeroMessageBox.Show(tb);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrimaryWindow.Content = new Pages.Home();
        }
    }
}
