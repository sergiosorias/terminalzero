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
            App.Instance.CurrentClient.Session.Notifier = this;
            App.Instance.CurrentClient.Session.ModuleList.ForEach(m => m.Notifing += new EventHandler<ModuleNotificationEventArgs>(m_Notifing));
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
            PrimaryWindow.Content = new Pages.Home();
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

        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private System.Windows.Forms.MenuItem m_notifyIconMenuItem;
        private System.Windows.Forms.MenuItem m_groupMenuItem;
        private void InitTryIcon()
        {
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipTitle = Properties.Settings.Default.ApplicationName;
            m_notifyIcon.BalloonTipClicked += (o, e) => { m_notifyIcon_Click(null, null); };
            m_notifyIcon.Text = "Terminal Zero";
            m_notifyIcon.Icon = Properties.Resources.ZeroAppIcon;
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
            m_notifyIcon.Visible = true;
            m_notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            m_notifyIconMenuItem = new System.Windows.Forms.MenuItem("Mostrar Notificaciones", (o, e) => { m_notifyIconMenuItem.Checked = !m_notifyIconMenuItem.Checked; });
            m_notifyIconMenuItem.Checked = true;
            m_notifyIcon.ContextMenu.MenuItems.Add(m_notifyIconMenuItem);

        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = System.Windows.WindowState.Maximized;
                Show();
            }

            Activate();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                WindowState = WindowState.Minimized;
                PopUpNotify("Terminal Zero esta minimizado, Click para abrir.");
            }
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }

        #endregion

        private void PopUpNotify(string text)
        {
            if (m_notifyIcon != null && m_notifyIconMenuItem != null && m_notifyIconMenuItem.Checked)
            {
                m_notifyIcon.BalloonTipText = text;
                m_notifyIcon.ShowBalloonTip(2000);
            }
        }

        private static TraceSwitch ZeroLogLevel = new TraceSwitch("ZeroLogLevelSwitch", "Zero Log Level Switch", "Error");
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
            System.Diagnostics.Trace.WriteLineIf(ZeroLogLevel.TraceVerbose, GetStamp() + LastMessage);
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
            System.Diagnostics.Trace.WriteLineIf(ZeroLogLevel.TraceInfo, GetStamp() + message);
        }

        public event EventHandler ExecutionFinished;

        public void NotifyExecutionFinished(object sender)
        {
            if (ExecutionFinished != null)
                ExecutionFinished(sender, EventArgs.Empty);
        }

        public void Log(TraceLevel level, string message)
        {
            System.Diagnostics.Trace.WriteLineIf(ZeroLogLevel.Level >= level, GetStamp() + message);
        }

        #endregion

    }
}
