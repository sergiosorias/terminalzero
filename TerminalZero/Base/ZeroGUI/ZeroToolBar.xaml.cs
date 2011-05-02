using System.Windows;
using System.Windows.Controls;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for ZeroToolBar.xaml
    /// </summary>
    public partial class ZeroToolBar : UserControl
    {
        public ZeroToolBar()
        {
            InitializeComponent();
            PrintBtnVisible = false;
        }

        public bool NewBtnVisible
        {
            get { return (bool)GetValue(NewBtnVisibleProperty); }
            set { SetValue(NewBtnVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewBtnVisibleProperty =
            DependencyProperty.Register("NewBtnVisible", typeof(bool), typeof(ZeroToolBar), new UIPropertyMetadata(true, OnNewBtnVisibleChanged));

        private static void OnNewBtnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as ZeroToolBar;
            if (tb != null)
            {
                tb.btnSaveLine.Visibility = tb.btnNew.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool SaveBtnVisible
        {
            get { return (bool)GetValue(SaveActiveProperty); }
            set {SetValue(SaveActiveProperty, value);}
        }

        // Using a DependencyProperty as the backing store for SaveActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveActiveProperty =
            DependencyProperty.Register("SaveBtnVisible", typeof(bool), typeof(ZeroToolBar), new UIPropertyMetadata(true,OnSaveBtnVisibleChanged));

        private static void OnSaveBtnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as ZeroToolBar;
            if(tb!=null)
            {
                tb.btnSave.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool CancelBtnVisible
        {
            get { return (bool)GetValue(CancelActiveProperty); }
            set {SetValue(CancelActiveProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CancelBtnVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelActiveProperty =
            DependencyProperty.Register("CancelBtnVisible", typeof(bool), typeof(ZeroToolBar), new UIPropertyMetadata(true,OnCancelBtnVisibleChanged));

        private static void OnCancelBtnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as ZeroToolBar;
            if (tb != null)
            {
                tb.btnCancel.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            }    
        }

        public bool PrintBtnVisible
        {
            get { return (bool)GetValue(PrintActiveProperty); }
            set
            {
                SetValue(PrintActiveProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for CancelBtnVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrintActiveProperty =
            DependencyProperty.Register("PrintBtnVisible", typeof(bool), typeof(ZeroToolBar), new UIPropertyMetadata(false, OnPrintBtnVisibleChanged));

        private static void OnPrintBtnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as ZeroToolBar;
            if (tb != null)
            {
                tb.btnPrint.Visibility = ((bool)e.NewValue) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public event RoutedEventHandler Save;

        private void OnSave(RoutedEventArgs e)
        {
            RoutedEventHandler handler = Save;
            if (handler != null) handler(this, e);
        }

        public event RoutedEventHandler Cancel;

        private void OnCancel(RoutedEventArgs e)
        {
            RoutedEventHandler handler = Cancel;
            if (handler != null) handler(this, e);
        }

        public event RoutedEventHandler New;

        private void OnNew(RoutedEventArgs e)
        {
            RoutedEventHandler handler = New;
            if (handler != null) handler(this, e);
        }

        public event RoutedEventHandler Print;

        private void OnPrint(RoutedEventArgs e)
        {
            RoutedEventHandler handler = Print;
            if (handler != null) handler(this, e);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            OnSave(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnCancel(e);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            OnNew(e);
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            OnPrint(e);
        }

   }
}
