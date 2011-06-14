using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for ZeroToolBar.xaml
    /// </summary>
    public partial class ZeroToolBar : UserControl
    {
        public static readonly DependencyProperty NewCommandProperty = DependencyProperty.RegisterAttached("NewCommand", typeof(ICommand), typeof(ZeroToolBar), new FrameworkPropertyMetadata(OnNewCommandChanged));

        public static ICommand GetNewCommand(NavigationBasePage control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            return control.GetValue(NewCommandProperty) as ICommand;
        }

        public static void SetNewCommand(NavigationBasePage control, ICommand command)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            control.SetValue(NewCommandProperty, command);
        }

        private static void OnNewCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NavigationBasePage)d;
            control.CommandBar.btnNew.Command = (ICommand)e.NewValue;
            
        }

        public static readonly DependencyProperty PrintCommandProperty = DependencyProperty.RegisterAttached("PrintCommand", typeof(ICommand), typeof(ZeroToolBar), new FrameworkPropertyMetadata(OnPrintCommandChanged));

        public static ICommand GetPrintCommand(NavigationBasePage control)
        {
            return control.GetValue(PrintCommandProperty) as ICommand;
        }

        public static void SetPrintCommand(NavigationBasePage control, ICommand command)
        {
            control.SetValue(PrintCommandProperty, command);
        }

        private static void OnPrintCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NavigationBasePage)d;
            control.CommandBar.btnPrint.Command = (ICommand)e.NewValue;
            //control.CommandBar.ShortCutPrint.Command = (ICommand) e.NewValue;
        }

        public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.RegisterAttached("SaveCommand", typeof(ICommand), typeof(ZeroToolBar), new FrameworkPropertyMetadata(OnSaveCommandChanged));

        public static ICommand GetSaveCommand(NavigationBasePage control)
        {
            return control.GetValue(SaveCommandProperty) as ICommand;
        }

        public static void SetSaveCommand(NavigationBasePage control, ICommand command)
        {
            control.SetValue(SaveCommandProperty, command);
        }

        private static void OnSaveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NavigationBasePage)d;
            control.CommandBar.btnSave.Command = (ICommand)e.NewValue;
            control.CommandBar.ShortCutAccept.Command = (ICommand) e.NewValue;
        }

        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.RegisterAttached("CancelCommand", typeof(ICommand), typeof(ZeroToolBar), new FrameworkPropertyMetadata(OnCancelCommandChanged));

        public static ICommand GetCancelCommand(NavigationBasePage control)
        {
            return control.GetValue(CancelCommandProperty) as ICommand;
        }

        public static void SetCancelCommand(NavigationBasePage control, ICommand command)
        {
            control.SetValue(CancelCommandProperty, command);
        }

        private static void OnCancelCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NavigationBasePage)d;
            control.CommandBar.btnCancel.Command = (ICommand)e.NewValue;
            control.CommandBar.ShortCutCancel.Command = (ICommand) e.NewValue;
        }

        //public static readonly RoutedEvent NewClickedEvent = EventManager.RegisterRoutedEvent("NewClick", RoutingStrategy.Direct,typeof(RoutedEventHandler), typeof(ZeroToolBar));
        
        public ZeroToolBar()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Save
        {
            add { btnSave.Click += value;
            btnSave.Command = new ZeroActionDelegate(Empty);
            }
            remove { btnSave.Click -= value; }
        }

        public event RoutedEventHandler Cancel
        {
            add { btnCancel.Click += value;
                btnCancel.Command = new ZeroActionDelegate(Empty);
            }
            remove { btnCancel.Click -= value; }
        }
        
        public event RoutedEventHandler New
        {
            add { btnNew.Click += value;
                btnNew.Command = new ZeroActionDelegate(Empty);
            }
            remove { btnNew.Click -= value; }
        }

        public event RoutedEventHandler Print
        {
            add { btnPrint.Click += value;
                btnPrint.Command = new ZeroActionDelegate(Empty);
            }
            remove { btnPrint.Click -= value; }
        }

        private static void Empty(object parameter)
        {
        }

        public void AppendButton(string content, RoutedEventHandler handler)
        {
            var newButton = new Button();
            newButton.Click += handler;
            newButton.Content = content;
            newButton.Style = (Style)Resources["toolbarButton"];

            AddSeparator();
            buttonsBar.Children.Add(newButton);

        }

        public void AppendButton(string content, ICommand command)
        {
            var newButton = new Button
            {
                Content = content,
                Style = (Style) Resources["toolbarButton"],
                Command = command
            };

            AddSeparator();
            buttonsBar.Children.Add(newButton);

        }

        private void AddSeparator()
        {
            var rect = new Rectangle
                                 {
                                     Style = (Style)Resources["separatorRectangle"]
                                 };
            buttonsBar.Children.Add(rect);
        }

        
   }
}
