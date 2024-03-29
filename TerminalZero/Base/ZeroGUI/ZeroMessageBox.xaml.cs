﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for ZeroMessageBox.xaml
    /// </summary>
    public partial class ZeroMessageBox : Window
    {
        private readonly bool isDialog;

        private ZeroActionDelegate acceptAction;
        public ZeroActionDelegate AcceptAction
        {
            get
            {
                return acceptAction ?? (acceptAction = new ZeroActionDelegate(Accept));
            }
        }

        private ZeroActionDelegate cancelAction;
        public ZeroActionDelegate CancelAction
        {
            get
            {
                return cancelAction ?? (cancelAction = new ZeroActionDelegate(Cancel));
            }
        }

        public ZeroMessageBox()
        {
            InitializeComponent();
            if (Application.Current.Windows.Count > 0)
            {
                Owner = Application.Current.Windows[Application.Current.Windows.Count - 2];

                if (Application.Current.Windows.Count > 0 && Application.Current.Windows[0] != null)
                {
                    MaxWidth = Application.Current.Windows[0].ActualWidth - 20;
                    MaxHeight = Application.Current.Windows[0].ActualHeight - 20;
                }
            }
        }

        private ZeroMessageBox(bool isDialog)
            : this()
        {
            this.isDialog = isDialog;
        }

        public new object Content
        {
            get
            {
                return contentpress.Content;
            }
            set
            {
                Control control;
                if (value is string)
                {
                    NavigationBasePage nvp = new NavigationBasePage();
                    var label = new TextBox();
                    label.BorderBrush = Brushes.Transparent;
                    label.Background = Brushes.Transparent;
                    label.TextAlignment = TextAlignment.Center;
                    label.IsReadOnly = true;
                    nvp.MaxWidth = 500;
                    nvp.Header = Title;
                    label.TextWrapping = TextWrapping.Wrap;
                    nvp.Content = label;
                    label.FontSize = 18;
                    label.FontWeight = FontWeights.Bold;
                    label.Text = (string)value;
                    control = nvp;
                    
                }
                else
                {
                    control = value as Control;
                }

                if (control != null)
                {
                    control.PreviewKeyDown += ZeroMessageBox_PreviewKeyDown;
                    control.MouseLeftButtonDown += CurrentMouseLeftButtonDown;
                    contentpress.Content = control;
                }
                else
                    contentpress.Content = value;

                
            }
        }

        private bool CanAccept(object parameter)
        {
            if (IsLoaded && Content is NavigationBasePage)
                return ((NavigationBasePage) Content).CanAccept(parameter);

            return true;
        }

        private void Accept(object parameter)
        {
            if (CanAccept(null))
            {
                Storyboard SB = (Storyboard)Resources["leaveStoryboard"];

                SB.Completed += (o, e) =>
                                    {
                                        try
                                        {
                                            if (isDialog)
                                                DialogResult = true;
                                        }
                                        catch
                                        {
                                        }

                                        Close();
                                    };
                SB.Begin();
            }
        }

        private bool CanCancel(object parameter)
        {
            if (IsLoaded && Content is NavigationBasePage)
                return ((NavigationBasePage)Content).CanCancel(parameter);

            return true;
        }

        private void Cancel(object parameter)
        {
            if (CanCancel(null))
            {
                Storyboard SB = (Storyboard)Resources["leaveStoryboard"];

                SB.Completed += (o, e) =>
                                    {
                                        try
                                        {
                                            if (isDialog)
                                                DialogResult = false;
                                        }
                                        catch
                                        {
                                        }


                                        Close();
                                    };
                SB.Begin();
            }
        }

        private void ZeroMessageBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter &&
                e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (AcceptAction.CanExecute(null))
                {
                    AcceptAction.Execute(null);
                }
            }
        }

        private void CurrentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CurrentLoaded(object sender, RoutedEventArgs e)
        {
            if (Content is NavigationBasePage)
            {
                KeyboardNavigation.SetControlTabNavigation((NavigationBasePage)Content, KeyboardNavigationMode.Continue);
                KeyboardNavigation.SetTabNavigation((NavigationBasePage)Content, KeyboardNavigationMode.Continue);
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else
            {
                btnAccept.Focus();
            }
        }

        #region Statics

        public static bool? Show(object content, ResizeMode resizeMode, MessageBoxButton mboxButtons)
        {
            return Show(content, "", resizeMode, mboxButtons);
        }

        public static bool? Show(object content, string caption)
        {
            return Show(content, caption, MessageBoxButton.OKCancel);
        }

        public static bool? Show(object content, string caption, MessageBoxButton mboxButtons)
        {
            return Show(content, caption, SizeToContent.WidthAndHeight, mboxButtons);
        }

        public static bool? Show(object content, string caption, ResizeMode resizeMode, MessageBoxButton mboxButtons)
        {
            return Show(content, caption, SizeToContent.WidthAndHeight, resizeMode, mboxButtons);
        }

        public static bool? Show(object content, string caption, SizeToContent sizeToContent)
        {
            return Show(content, caption, sizeToContent, MessageBoxButton.OKCancel);
        }

        public static bool? Show(object content, string caption, SizeToContent sizeToContent, MessageBoxButton mboxButtons)
        {
            return Show(content, caption, sizeToContent, ResizeMode.CanResize, mboxButtons);
        }

        public static bool? Show(object content, string caption, SizeToContent sizeToContent, ResizeMode resizeMode, MessageBoxButton mboxButtons)
        {
            var MB = new ZeroMessageBox(true);
            if (caption != null) MB.Title = caption;
            MB.Content = content;
            MB.ResizeMode = resizeMode;
            switch (mboxButtons)
            {
                case MessageBoxButton.OK:
                    MB.btnCancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    MB.btnCancel.Content = "No";
                    MB.btnAccept.Content = "Si";
                    break;
            }
            MB.SizeToContent = sizeToContent;
            object obj = Application.Current.Windows[0].Content;

            if (obj is Panel)
                Terminal.Instance.Client.ShowEnable(false);

            bool? res = MB.ShowDialog();

            if (obj is Panel)
                Terminal.Instance.Client.ShowEnable(true);

            return res;
        }

        #endregion Statics

    }
}
