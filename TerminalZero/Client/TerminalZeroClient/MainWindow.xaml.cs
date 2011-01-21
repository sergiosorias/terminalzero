﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using System.Collections.Generic;

namespace TerminalZeroClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IProgressNotifier
    {
        private object _lastViewShown = null;
        public MainWindow()
        {
            InitializeComponent();
            Messages = new Queue<string>(10);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTryIcon();
            LoadConfigs();
            App.Instance.Manager.ConfigurationRequired += Manager_ConfigurationRequired;
            App.Instance.Session.Notifier = this;
            App.Instance.Session.ModuleList.ForEach(m => m.Notifing += m_Notifing);
            try
            {
                App.Instance.Session.ModuleList.ForEach(m => m.Init());
            }
            catch (Exception ex)
            {
                SendNotification(string.Format("Error on Init. Error: {0}",ex));
            }
            OpenHome();
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
            item.Header = Properties.Resources.Home;
            ZeroAction actionInit = null;
            if (!App.Instance.Manager.ExistsAction(ApplicationActions.Home, out actionInit))
            {
                actionInit = new ZeroAction(null,ActionType.BackgroudAction, ApplicationActions.Home,OpenHome);
                App.Instance.Session.AddAction(actionInit);
                App.Instance.Session.AddAction(new ZeroAction(null, ActionType.BackgroudAction, ApplicationActions.Back, GoBack));
                App.Instance.Session.AddAction(new ZeroAction(null, ActionType.BackgroudAction, ApplicationActions.Exit, ForceClose));
            }
            ShortCutHome.Command = actionInit;
            item.Command = actionInit;
            mainMenu.Items.Add(item);
            InternalBuildMenu(App.Instance.CurrentClient.BuildMenu(), mainMenu.Items);
        }
        
        private void OpenHome()
        {
            ModuleNotificationEventArgs args = new ModuleNotificationEventArgs();
            args.ControlToShow = new Pages.Home();
            m_Notifing(null, args);
        }

        private void GoBack()
        {
            ModuleNotificationEventArgs args = new ModuleNotificationEventArgs();
            args.ControlToShow = _lastViewShown;
            m_Notifing(null, args);
        }

        private void m_Notifing(object sender, ModuleNotificationEventArgs e)
        {
            if (e.SomethingToShow)
            {
                if (e.ControlToShow is ZeroGUI.ZeroMessageBox)
                {
                    ((ZeroMessageBox) e.ControlToShow).Owner = this;
                    ((ZeroMessageBox) e.ControlToShow).Top = Top + 1;
                    ((ZeroMessageBox) e.ControlToShow).ShowDialog();
                }
                else if (!(PrimaryWindow.Content is IZeroPage))
                {
                    ShowObject(e);
                }
                else if (((IZeroPage) PrimaryWindow.Content).CanAccept(null))
                {
                    ShowObject(e);
                }
                
            }
        }

        private void ShowObject(ModuleNotificationEventArgs e)
        {
            _lastViewShown = PrimaryWindow.Content;

            PrimaryWindow.Content = e.ControlToShow;
            if (e.ControlToShow is UIElement)
            {
                bool focus = ((UIElement)e.ControlToShow).Focus();
                if (focus) ((UIElement) e.ControlToShow).CaptureMouse();
            }
        }

        private static void InternalBuildMenu(TerminalZeroClient.Extras.ZeroMenu menu, ItemCollection items)
        {
            foreach (var item in menu)
            {
                MenuItem menuitem = new MenuItem();
                menuitem.Header = item.Key;
                if (item.Value.Count > 0)
                    InternalBuildMenu(item.Value, menuitem.Items);
                else
                {
                    menuitem.Command = item.Value.MenuAction;
                }

                items.Add(menuitem);
            }
        }

        private void btnGetMoreStatusInfo_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = new TextBox();
            tb.Text = "";
            while (Messages.Count>0)
            {
                tb.Text += Environment.NewLine + Messages.Dequeue();
            }
            
            btnGetMoreStatusInfo.Visibility = System.Windows.Visibility.Hidden;
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tb.TextWrapping = TextWrapping.Wrap;
            ZeroMessageBox.Show(tb, "Información");
        }

        #region TryIcon

        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private System.Windows.Forms.MenuItem _notifyIconMenuItem;
        
        private bool _isNotifyIconOpen = false;
        private void InitTryIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.BalloonTipTitle = Properties.Settings.Default.ApplicationName;
            _notifyIcon.BalloonTipClicked += (o, e) => { m_notifyIcon_Click(null, null); };
            _notifyIcon.Text = App.Instance.Name;
            _notifyIcon.Icon = Properties.Resources.ZeroAppIcon;
            _notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            _notifyIcon.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            _notifyIconMenuItem = new System.Windows.Forms.MenuItem("Mostrar Notificaciones", (o, e) => { _notifyIconMenuItem.Checked = !_notifyIconMenuItem.Checked; });
            _notifyIconMenuItem.Checked = true;
            _notifyIcon.ContextMenu.MenuItems.Add(_notifyIconMenuItem);

        }

        private void ContextMenu_Popup(object sender, EventArgs e)
        {
            _isNotifyIconOpen = !_isNotifyIconOpen;
        }

        private void m_notifyIcon_Click(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && !_isNotifyIconOpen)
            {
                WindowState = System.Windows.WindowState.Maximized;
                Show();
            }
            if(_isNotifyIconOpen)
                _isNotifyIconOpen = false;

            Activate();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                WindowState = WindowState.Minimized;
                PopUpNotify(Properties.Resources.AppMinimized + ", " + Properties.Resources.ClickToOpen);
            }
        }
        
        #endregion

        private bool _isForced = false;
        private void ForceClose()
        {
            _isForced = true;
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isForced)
            {
                var res = MessageBox.Show(Properties.Resources.QuestionAreYouSureAppClosing,
                                          Properties.Resources.Closing, MessageBoxButton.YesNoCancel,
                                          MessageBoxImage.Question);
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
                        _notifyIcon.Dispose();
                        _notifyIcon = null;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }

        }

        private void PopUpNotify(string text)
        {
            if (_notifyIcon != null && _notifyIconMenuItem != null && _notifyIconMenuItem.Checked)
            {
                _notifyIcon.BalloonTipText = text;
                _notifyIcon.ShowBalloonTip(2000);
            }
        }

        public string LastMessage { get; set; }
        public Queue<string> Messages { get; private set; }
        public int MaxSaveMessages { get; private set; }

        #region IProgressNotifier Members

        public void SetProcess(string newProgress)
        {
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate() { statusMsg.Content = newProgress; }), null);
            Messages.Enqueue(GetStamp() + newProgress);
        }

        private static string GetStamp()
        {
            return DateTime.Now.ToString("MM/dd hh:mm:ss") + " - ";
        }

        public void SetProgress(int newProgress)
        {
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate() { statusBar.Value = newProgress; }), null);
        }

        public void SetUserMessage(bool isMandatory, string message)
        {
            this.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate()
            {
                btnGetMoreStatusInfo.Visibility = isMandatory ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }), null);

            LastMessage = message;
            this.Messages.Enqueue(GetStamp() + LastMessage);
            Log(TraceLevel.Verbose, GetStamp() + LastMessage);
        }

        public void SendNotification(string message)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork+=(sender, args)=>{
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
            };

            worker.RunWorkerAsync();
        }

        public void Log(TraceLevel level, string message)
        {
            Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.Level >= level, GetStamp() + message);
        }

        #endregion
    }
}