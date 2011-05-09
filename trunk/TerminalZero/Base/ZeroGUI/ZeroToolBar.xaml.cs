using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

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
        }

        public bool NewBtnVisible
        {
            get { return (bool)GetValue(NewBtnVisibleProperty); }
            set { SetValue(NewBtnVisibleProperty, value); }
        }

        public static readonly DependencyProperty NewBtnVisibleProperty =
            DependencyProperty.Register("NewBtnVisible", typeof(bool), typeof(ZeroToolBar), null);

        public ICommand NewBtnCommand
        {
            get { return btnNew.Command; }
            set { btnNew.Command = value; }
        }

        public bool SaveBtnVisible
        {
            get { return (bool)GetValue(SaveActiveProperty); }
            set {SetValue(SaveActiveProperty, value);}
        }

        public static readonly DependencyProperty SaveActiveProperty =
            DependencyProperty.Register("SaveBtnVisible", typeof(bool), typeof(ZeroToolBar),null);

        public ICommand SaveBtnCommand
        {
            get { return btnSave.Command; }
            set { btnSave.Command = value; }
        }

        public bool CancelBtnVisible
        {
            get { return (bool)GetValue(CancelActiveProperty); }
            set {SetValue(CancelActiveProperty, value);
            }
        }

        public static readonly DependencyProperty CancelActiveProperty =
            DependencyProperty.Register("CancelBtnVisible", typeof(bool), typeof(ZeroToolBar),null);

        public ICommand CancelBtnCommand
        {
            get { return btnCancel.Command; }
            set { btnCancel.Command = value; }
        }

        public bool PrintBtnVisible
        {
            get { return (bool)GetValue(PrintActiveProperty); }
            set
            {
                SetValue(PrintActiveProperty, value);
            }
        }

        public static readonly DependencyProperty PrintActiveProperty =
            DependencyProperty.Register("PrintBtnVisible", typeof(bool), typeof(ZeroToolBar), null);

        public ICommand PrintBtnCommand
        {
            get { return btnPrint.Command; }
            set { btnPrint.Command = value; }
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

        public void AppendButton(string content, RoutedEventHandler handler)
        {
            Button newButton = new Button();
            newButton.Click += handler;
            newButton.Content = content;
            newButton.Style = (Style)Resources["toolbarButton"];

            AddSeparator();
            buttonsBar.Children.Add(newButton);

        }

        public void AppendButton(string content, ICommand command)
        {
            Button newButton = new Button();
            newButton.Content = content;
            newButton.Style = (Style)Resources["toolbarButton"];

            newButton.Command = command;
            AddSeparator();
            buttonsBar.Children.Add(newButton);

        }

        private void AddSeparator()
        {
            Rectangle rect = new Rectangle()
                                 {
                                     Style = (Style)Resources["separatorRectangle"]
                                 };
            buttonsBar.Children.Add(rect);
        }

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SaveBtnVisible = Save != null || SaveBtnCommand!=null;
            CancelBtnVisible = Cancel != null || CancelBtnCommand != null;
            NewBtnVisible = New != null || NewBtnCommand != null;
            PrintBtnVisible = Print != null || PrintBtnCommand != null;
                 
        }

   }
}
