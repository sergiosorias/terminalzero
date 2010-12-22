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
using System.Drawing;
using System.Diagnostics;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IProgressNotifier
    {
        public MainWindow()
        {
            InitializeComponent();
            Messages = new StringBuilder();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.Instance.CurrentClient.Manager.ConfigurationRequired += new EventHandler(Manager_ConfigurationRequired);
            App.Instance.Session.Notifier = this;
            App.Instance.Session.ModuleList.ForEach(m => m.Notifing += new EventHandler<ModuleNotificationEventArgs>(m_Notifing));
            LoadConfigs();
            InitTryIcon();
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
            item.Click += menuitemitem_Click;
            ZeroAction actionInit = new ZeroAction(ActionType.MenuItem, "Home", OpenHome);
            item.DataContext = actionInit;
            mainMenu.Items.Add(item);
            InternalBuildMenu(App.Instance.CurrentClient.BuildMenu(), mainMenu.Items);
        }

        private void OpenHome(ZeroRule rule)
        {
            ModuleNotificationEventArgs args = new ModuleNotificationEventArgs();
            args.ControlToShow = new Pages.Home();
            m_Notifing(null, args);
        }

        private void m_Notifing(object sender, ModuleNotificationEventArgs e)
        {
            if (e.SomethingToShow)
            {
                if (PrimaryWindow.Content is IZeroPage && ((IZeroPage)PrimaryWindow.Content).CanAccept())
                {
                    PrimaryWindow.Content = e.ControlToShow;
                }
                else
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
                    if (!item.Value.MenuAction.AlwaysVisible)
                        menuitem.SetBinding(MenuItem.IsEnabledProperty, "Enabled");

                    if (item.Value.MenuAction.RuleToSatisfy != null && !item.Value.MenuAction.Enabled)
                    {
                        ToolTipService.SetToolTip(menuitem, item.Value.MenuAction.RuleToSatisfy.Result);
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

            mainBar.IsEnabled = true;
        }

        private void btnGetMoreStatusInfo_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = new TextBox();
            tb.Text = Messages.ToString();
            Messages.Clear();
            btnGetMoreStatusInfo.Visibility = System.Windows.Visibility.Hidden;
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tb.TextWrapping = TextWrapping.Wrap;
            ZeroMessageBox.Show(tb);
        }

        #region TryIcon

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.MenuItem notifyIconMenuItem;
        
        private bool isNotifyIconOpen = false;
        private void InitTryIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.BalloonTipTitle = Properties.Settings.Default.ApplicationName;
            notifyIcon.BalloonTipClicked += (o, e) => { m_notifyIcon_Click(null, null); };
            notifyIcon.Text = App.Instance.Name;
            notifyIcon.Icon = Properties.Resources.ZeroAppIcon;
            notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
            notifyIcon.Visible = true;
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            notifyIcon.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            notifyIconMenuItem = new System.Windows.Forms.MenuItem("Mostrar Notificaciones", (o, e) => { notifyIconMenuItem.Checked = !notifyIconMenuItem.Checked; });
            notifyIconMenuItem.Checked = true;
            notifyIcon.ContextMenu.MenuItems.Add(notifyIconMenuItem);

        }

        private void ContextMenu_Popup(object sender, EventArgs e)
        {
            isNotifyIconOpen = !isNotifyIconOpen;
        }

        private void m_notifyIcon_Click(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && !isNotifyIconOpen)
            {
                WindowState = System.Windows.WindowState.Maximized;
                Show();
            }
            if(isNotifyIconOpen)
                isNotifyIconOpen = false;

            Activate();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                WindowState = WindowState.Minimized;
                PopUpNotify("Aplicación minizada, Click para abrir.");
            }
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var res = MessageBox.Show("¿Esta seguro que desea salir de la aplicacion?","Cerrando",MessageBoxButton.YesNoCancel,MessageBoxImage.Question);
            switch (res)
            {
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
                case MessageBoxResult.No:
                    e.Cancel = true;
                    WindowState = WindowState.Minimized;
                    break;
                case MessageBoxResult.None:
                    break;
                case MessageBoxResult.OK:
                case MessageBoxResult.Yes:
                    notifyIcon.Dispose();
                    notifyIcon = null;
                    break;
                default:
                    break;
            }
            
        }

        #endregion

        private void PopUpNotify(string text)
        {
            if (notifyIcon != null && notifyIconMenuItem != null && notifyIconMenuItem.Checked)
            {
                notifyIcon.BalloonTipText = text;
                notifyIcon.ShowBalloonTip(2000);
            }
        }

        public string LastMessage { get; set; }
        public StringBuilder Messages { get; private set; }

        #region IProgressNotifier Members

        public void SetProcess(string newProgress)
        {
            this.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate() { statusMsg.Content = newProgress; }), null);
            this.Messages.AppendLine(GetStamp() + newProgress);
        }

        private static string GetStamp()
        {
            return DateTime.Now.ToString("hh:mm:ss") + " - ";
        }

        public void SetProgress(int newProgress)
        {
            this.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate() { statusBar.Value = newProgress; }), null);
        }

        public void SetUserMessage(bool isMandatory, string message)
        {
            this.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                if (isMandatory)
                {
                    btnGetMoreStatusInfo.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    btnGetMoreStatusInfo.Visibility = System.Windows.Visibility.Hidden;
            }), null);

            LastMessage = message;
            this.Messages.AppendLine(GetStamp() + LastMessage);
            Log(TraceLevel.Verbose, GetStamp() + LastMessage);
        }

        public void SendNotification(string message)
        {
            this.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate()
                {
                    if (WindowState != System.Windows.WindowState.Minimized)
                    {
                        System.Windows.MessageBox.Show(message, "Informacion importante", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        PopUpNotify(message);
                    }
                }
                ), null);
            Log(TraceLevel.Info, GetStamp() + message);
        }

        public void Log(TraceLevel level, string message)
        {
            System.Diagnostics.Trace.WriteLineIf(App.Instance.CurrentClient.LogLevel.Level >= level, GetStamp() + message);
        }

        #endregion

    }
}
