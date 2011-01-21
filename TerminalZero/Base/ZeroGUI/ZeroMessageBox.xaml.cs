using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for ZeroMessageBox.xaml
    /// </summary>
    public partial class ZeroMessageBox : Window
    {
        bool _isDialog = false;
        private readonly ZeroAction _acceptAction;
        private readonly ZeroAction _cancelAction;
        public ZeroMessageBox()
        {
            InitializeComponent();
            Owner = Application.Current.Windows[Application.Current.Windows.Count-2];
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            lblCaption.DataContext = this;
            _acceptAction = new ZeroAction(null, ActionType.BackgroudAction, "cancel", Accept);
            _cancelAction = new ZeroAction(null, ActionType.BackgroudAction, "accept", Cancel);
            btnAccept.Command = ShortCutAccept.Command = _acceptAction;
            btnCancel.Command = ShortCutCancel.Command = _cancelAction;
            
        }

        public ZeroMessageBox(bool isDialog)
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
                if (Content is IZeroPage)
                {
                    _acceptAction.RuleToSatisfy = ((IZeroPage)value).CanAccept;
                    _cancelAction.RuleToSatisfy = ((IZeroPage)value).CanCancel;
                }
            }
        }

        private void Accept()
        {
            if (_isDialog)
                DialogResult = true;

            this.Close();
        }

        private void Cancel()
        {
            if (_isDialog)
                DialogResult = false;

            this.Close();
        }

        private void CurrentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CurrentLoaded(object sender, RoutedEventArgs e)
        {
            if (Content is IZeroPage)
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
            return Show(content, "", SizeToContent.WidthAndHeight);
        }

        public static bool? Show(object content, ResizeMode resizeMode)
        {
            return Show(content, "", resizeMode);
        }

        public static bool? Show(object content, string caption)
        {
            return Show(content, caption, SizeToContent.WidthAndHeight);
        }

        public static bool? Show(object content, string caption, ResizeMode resizeMode)
        {
            return Show(content, caption, SizeToContent.WidthAndHeight,resizeMode);
        }

        public static bool? Show(object content, string caption, SizeToContent sizeToContent)
        {
            return Show(content, caption, sizeToContent, ResizeMode.CanResize);
        }

        public static bool? Show(object content, string caption, SizeToContent sizeToContent, ResizeMode resizeMode)
        {
            ZeroMessageBox MB = new ZeroMessageBox(true);
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
            MB.Title = caption;
            MB.SizeToContent = sizeToContent;
            return MB.ShowDialog();
        }

        #endregion Statics

    }
}
