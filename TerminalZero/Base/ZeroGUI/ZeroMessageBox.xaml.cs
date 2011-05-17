using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZeroCommonClasses.GlobalObjects.Actions;

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
            _acceptAction = new ZeroBackgroundAction("cancel", Accept, null,false,false);
            _cancelAction = new ZeroBackgroundAction("accept", Cancel, null, false, false);
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
                if(Content is Control)
                {
                    ((Control)Content).PreviewKeyDown += ZeroMessageBox_PreviewKeyDown;
                    ((Control)Content).HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    ((Control)Content).VerticalContentAlignment = VerticalAlignment.Stretch;
                }
                if(Content is NavigationBasePage)
                {
                    _acceptAction.RuleToSatisfy = ((NavigationBasePage)value).CanAccept;
                    _cancelAction.RuleToSatisfy = ((NavigationBasePage)value).CanCancel;
                    lblCaption.Visibility = Visibility.Collapsed;
                    if (ResizeMode == ResizeMode.NoResize)
                    {
                        Background = Brushes.Transparent;
                        AllowsTransparency = true;
                    }
                    ((NavigationBasePage) value).MouseLeftButtonDown += CurrentMouseLeftButtonDown;
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
            if (Content is NavigationBasePage)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
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
            MB.Title = caption;
            MB.SizeToContent = sizeToContent;
            object obj = Application.Current.Windows[0].Content;
            
            if(obj is Panel)
                ((Panel)obj).Children.Add((UIElement)MB.Resources["backWindow"]);
            
            bool? res = MB.ShowDialog();
            
            if (obj is Panel)
                ((Panel)obj).Children.Remove((UIElement)MB.Resources["backWindow"]);

            return res;
        }

        #endregion Statics
        
    }
}
