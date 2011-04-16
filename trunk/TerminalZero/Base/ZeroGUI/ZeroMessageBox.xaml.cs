﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZeroBusiness;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for ZeroMessageBox.xaml
    /// </summary>
    public partial class ZeroMessageBox : Window
    {
        bool _isDialog;

        private readonly ZeroAction _acceptAction;
        private readonly ZeroAction _cancelAction;

        public ZeroMessageBox()
        {
            InitializeComponent();
            if (Application.Current.Windows.Count > 0)
            {
                Owner = Application.Current.Windows[Application.Current.Windows.Count - 2];
                
                MaxWidth = Application.Current.Windows[0].ActualWidth;
                MaxHeight = Application.Current.Windows[0].ActualHeight;
                
            }
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            lblCaption.DataContext = this;
            _acceptAction = new ZeroAction( ActionType.BackgroudAction, "cancel", Accept);
            _cancelAction = new ZeroAction( ActionType.BackgroudAction, "accept", Cancel);
            btnAccept.Command = ShortCutAccept.Command = _acceptAction;
            btnCancel.Command = ShortCutCancel.Command = _cancelAction;
            
        }

        private ZeroMessageBox(bool isDialog)
            :this()
        {
            _isDialog = isDialog;
        }

        public new object Content
        {
            get
            {
                return contentpress.Content;
            }

            set
            {
                contentpress.Content = value;
                if(Content is UserControl)
                {
                    ((UserControl) Content).PreviewKeyDown += ZeroMessageBox_PreviewKeyDown;
                    ((UserControl)Content).HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    ((UserControl)Content).VerticalContentAlignment = VerticalAlignment.Stretch;
                }
                if(Content is ZeroBasePage)
                {
                    _acceptAction.RuleToSatisfy = ((ZeroBasePage)value).CanAccept;
                    _cancelAction.RuleToSatisfy = ((ZeroBasePage)value).CanCancel;
                }
            }
        }

        private void Accept()
        {
            try
            {
            if (_isDialog)
                DialogResult = true;
            }
            catch { }

            Close();
        }

        private void Cancel()
        {
            try
            {
                if (_isDialog)
                    DialogResult = false;
            }
            catch{}
            

            Close();
        }

        private void ZeroMessageBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter &&
                e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (_acceptAction.CanExecute(null))
                {
                    _acceptAction.Execute(null);
                }
            }
        }

        private void CurrentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CurrentLoaded(object sender, RoutedEventArgs e)
        {
            if (Content is ZeroBasePage)
            {
                
            }
            else
            {
                btnAccept.Focus();
            }
        }

        #region Statics

        public static bool? Show(object content)
        {
            return Show(content, MessageBoxButton.OK);
        }

        public static bool? Show(object content, MessageBoxButton mboxButtons)
        {
            return Show(content, "", SizeToContent.WidthAndHeight,mboxButtons);
        }

        public static bool? Show(object content, ResizeMode resizeMode)
        {
            return Show(content, resizeMode, MessageBoxButton.OKCancel);
        }

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

        public static bool? Show(object content, string caption, ResizeMode resizeMode)
        {
            return Show(content, caption, resizeMode, MessageBoxButton.OKCancel);
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
            MB.Content = content;
            MB.ResizeMode = resizeMode;
            if(resizeMode == ResizeMode.NoResize)
            {
                MB.BorderThickness = new Thickness(1);
                MB.BorderBrush = Brushes.Black;
            }
            if (content is UIElement)
            {
                ((UIElement)content).Focus();
            }

            switch (mboxButtons)
            {
                case MessageBoxButton.OK:
                    MB.btnCancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                   break;
                case MessageBoxButton.YesNoCancel:
                    break;
                case MessageBoxButton.YesNo:
                     MB.btnCancel.Content = "No";
                     MB.btnAccept.Content = "Si";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mboxButtons");
            }
            MB.Title = caption;
            MB.SizeToContent = sizeToContent;
            return MB.ShowDialog();
        }

        #endregion Statics
        
    }
}