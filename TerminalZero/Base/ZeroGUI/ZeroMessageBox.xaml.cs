using System.Windows;
using System.Windows.Input;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for ZeroMessageBox.xaml
    /// </summary>
    public partial class ZeroMessageBox : Window
    {
        bool _isDialog = false;
        public ZeroMessageBox()
        {
            InitializeComponent();
            Owner = Application.Current.Windows[0];
            lblCaption.DataContext = this;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (Content is UIElement)
            {
                UIElement elem = Content as UIElement;
                elem.KeyDown -= new KeyEventHandler(ElemKeyDown);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            ElemKeyDown(null, e);
            base.OnKeyDown(e);
        }

        private void ElemKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CancelButtonClick(null, null);
            }
            else if(e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                e.Handled = true;
                AcceptButtonClick(null, null);
            }
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
                if (Content is UIElement)
                {
                    UIElement elem = Content as UIElement;
                    elem.PreviewKeyDown += new KeyEventHandler(ElemKeyDown);
                }
            }
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs e)
        {
            bool exit = true;

            if (contentpress.Content is IZeroPage)
            {
                exit = ((IZeroPage)contentpress.Content).CanAccept();
            }

            if (_isDialog && exit)
                DialogResult = true;

            if(exit)
                this.Close();


        }

        private void CurrentMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            bool exit = true;

            if (contentpress.Content is IZeroPage)
            {
                exit = ((IZeroPage)contentpress.Content).CanCancel();
            }

            if (_isDialog && exit)
                DialogResult = false;

            if (exit)
                this.Close();
        }

        public static bool? Show(object content)
        {
            return Show(content, "", SizeToContent.WidthAndHeight);
        }

        public static bool? Show(object content, string caption)
        {
            return Show(content, caption, SizeToContent.WidthAndHeight);
        }

        public static bool? Show(object content, SizeToContent sizeToContent)
        {
            return Show(content, "", sizeToContent);
        }

        public static bool? Show(object content, string caption, SizeToContent sizeToContent)
        {
            ZeroMessageBox MB = new ZeroMessageBox();
            MB.Content = content;
            if (content is UIElement)
            {
                ((UIElement)content).Focus();
            }
            MB.Title = caption;
            MB.SizeToContent = sizeToContent;
            MB._isDialog = true;
            return MB.ShowDialog();
        }
    }
}
