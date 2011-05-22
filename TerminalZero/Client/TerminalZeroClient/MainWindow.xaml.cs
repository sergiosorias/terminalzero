﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using TerminalZeroClient.Pages;
using TerminalZeroClient.Properties;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

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
            Messages = new Queue<string>(10);
        }

        public ZeroAction GoHome
        {
            get
            {
                return new ZeroBackgroundAction(Actions.AppHome, OpenHome, null, false, false);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTryIcon();
            LoadConfigs();
            Terminal.Instance.Manager.ConfigurationRequired += Manager_ConfigurationRequired;
            Terminal.Instance.CurrentClient.Notifier = this;
            try
            {
                Terminal.Instance.CurrentClient.ModuleList.ForEach(m => m.Init());
            }
            catch (Exception ex)
            {
                SendNotification(string.Format("Error on Init. Error: {0}",ex));
            }
            
        }

        void Manager_ConfigurationRequired(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new MethodInvoker(LoadConfigs), null);
        }

        private void LoadConfigs()
        {
            object item = mainMenu.Items[0];
            mainMenu.Items.Clear();
            mainMenu.Items.Add(item);
            if (!Terminal.Instance.Session.Actions.Exists(Actions.AppHome))
            {
                Terminal.Instance.Session.Actions.Add(GoHome);
                Terminal.Instance.Session.Actions.Add(new ZeroBackgroundAction(Actions.AppExit, ForceClose, null, false, false));
            }
            InternalBuildMenu(Terminal.Instance.CurrentClient.MainMenu, mainMenu.Items);
        }
        
        private static void OpenHome()
        {
            Terminal.Instance.CurrentClient.ShowView(new Home());
        }

        private void InternalBuildMenu(ZeroMenu menu, ItemCollection items)
        {
            foreach (var item in menu)
            {
                var menuitem = new System.Windows.Controls.MenuItem();
                menuitem.Header = item.Key;
                if (item.Value.Count > 0)
                    InternalBuildMenu(item.Value, menuitem.Items);
                else
                {
                    menuitem.Command = item.Value.MenuAction;
                    //Esto hay que hacerlo algun dia!
                    //menuitem.ContextMenu = new System.Windows.Controls.ContextMenu();
                    //menuitem.ContextMenu.Items.Add(new System.Windows.Controls.MenuItem {Header = "Anclar al Inicio", IsCheckable=true});

                }

                items.Add(menuitem);
            }
        }

        private void btnGetMoreStatusInfo_Click(object sender, RoutedEventArgs e)
        {
            var tb = new TextBox();
            tb.Text = "";
            while (Messages.Count>0)
            {
                tb.Text += Environment.NewLine + Messages.Dequeue();
            }
            
            btnGetMoreStatusInfo.Visibility = Visibility.Hidden;
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            tb.TextWrapping = TextWrapping.Wrap;
            ZeroMessageBox.Show(tb, "Información",ResizeMode.CanResize,MessageBoxButton.OK);
        }

        #region TryIcon

        private NotifyIcon _notifyIcon;
        private bool _isNotifyIconOpen;
        private void InitTryIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.BalloonTipTitle = Settings.Default.ApplicationName;
            _notifyIcon.BalloonTipClicked += m_notifyIcon_Click;
            _notifyIcon.Text = App.Name;
            _notifyIcon.Icon = Properties.Resources.ZeroAppIcon;
            _notifyIcon.Click += m_notifyIcon_Click;
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenu = new ContextMenu();
            _notifyIcon.ContextMenu.Popup += ContextMenu_Popup;
            var notifyIconMenuItem = new MenuItem("Mostrar Notificaciones",
            (o, e) =>
                {
                    Settings.Default.ShowNotifications = !Settings.Default.ShowNotifications;
                    ((MenuItem)o).Checked = Settings.Default.ShowNotifications;
                    Settings.Default.Save();
                }) { Checked = Settings.Default.ShowNotifications };
            var item = new MenuItem("Preguntar al cerrar", (o, e) =>
                                                                    {
                Settings.Default.AskForClose = !Settings.Default.AskForClose;
                ((MenuItem)o).Checked = Settings.Default.AskForClose;
                Settings.Default.Save();
            }) { Checked = Settings.Default.AskForClose };
            _notifyIcon.ContextMenu.MenuItems.Add(notifyIconMenuItem);
            _notifyIcon.ContextMenu.MenuItems.Add(item);

        }

        private void ContextMenu_Popup(object sender, EventArgs e)
        {
            _isNotifyIconOpen = !_isNotifyIconOpen;
        }

        private void m_notifyIcon_Click(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized && !_isNotifyIconOpen)
            {
                WindowState = WindowState.Maximized;
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

        private bool _isForced;
        private void ForceClose()
        {
            _isForced = true;
            Close();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Settings.Default.AskForClose && !_isForced)
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
            if (_notifyIcon != null && Settings.Default.ShowNotifications)
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
            Dispatcher.Invoke(new MethodInvoker(delegate { statusMsg.Content = newProgress; }), null);
            Messages.Enqueue(GetStamp() + newProgress);
        }

        private static string GetStamp()
        {
            return DateTime.Now.ToString("MM/dd hh:mm:ss") + " - ";
        }

        public void SetProgress(int newProgress)
        {
            Dispatcher.Invoke(new MethodInvoker(delegate { statusBar.Value = newProgress; }), null);
        }

        public void SetUserMessage(bool isMandatory, string message)
        {
            Dispatcher.Invoke(new MethodInvoker(delegate {
                btnGetMoreStatusInfo.Visibility = isMandatory ? Visibility.Visible : Visibility.Hidden;
            }), null);

            LastMessage = message;
            Messages.Enqueue(GetStamp() + LastMessage);
            Log(TraceLevel.Verbose, GetStamp() + LastMessage);
        }

        public void SendNotification(string message)
        {
            var worker = new BackgroundWorker();
            worker.DoWork+=(sender, args)=>{
            Dispatcher.Invoke(new MethodInvoker(delegate
                                                                         {
                    if (WindowState != WindowState.Minimized)
                    {
                        MessageBox.Show(message, "Informacion importante", MessageBoxButton.OK, MessageBoxImage.Information);
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
            Trace.WriteLineIf(ConfigurationContext.LogLevel.Level >= level, GetStamp() + message);
        }

        #endregion
    }
}
