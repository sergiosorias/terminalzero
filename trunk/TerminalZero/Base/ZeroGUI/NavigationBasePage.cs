using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroGUI.Classes;

namespace ZeroGUI
{
    public class NavigationBasePage : HeaderedContentControl, IValidable
    {
        protected bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(this); }
        }

        static NavigationBasePage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBasePage),
                new FrameworkPropertyMetadata(typeof(NavigationBasePage)));
        }

        public NavigationBasePage()
        {
            CommandBar = new ZeroToolBar();
        }

        private bool _focusOnError = false;
        public bool FocusOnError
        {
            get { return _focusOnError; }
            set { _focusOnError = value; }
        }

        public ViewModelGui ViewModel
        {
            get { return DataContext as ViewModelGui; } 
            set { DataContext = value; }
        }

        public ControlMode ControlMode
        {
            get { return (ControlMode)GetValue(ModeProperty); }
            set
            {
                if (ControlMode != value)
                {
                    SetValue(ModeProperty, value);
                    OnControlModeChanged(value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for ControlMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("ControlMode", typeof(ControlMode), typeof(NavigationBasePage), new PropertyMetadata(ControlMode.NotSet));

        public ZeroToolBar CommandBar
        {
            get { return (ZeroToolBar)GetValue(ToolBarProperty); }
            set { SetValue(ToolBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolBarProperty =
            DependencyProperty.Register("CommandBar", typeof(ZeroToolBar), typeof(NavigationBasePage), null);
        
        public bool StartKeyRequired
        {
            get { return (bool)GetValue(StartKeyRequiredProperty); }
            set
            {
                SetValue(StartKeyRequiredProperty, false);
                SetValue(StartKeyRequiredProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for StartKeyRequired.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartKeyRequiredProperty =
            DependencyProperty.Register("StartKeyRequired", typeof(bool), typeof(NavigationBasePage), new UIPropertyMetadata(false));

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ControlMode = ControlMode.New;
        }

        public virtual bool CanAccept(object parameter)
        {
            bool ret = Validate();
            if(!ret)
            {
                ZeroMessageBox.Show("Por favor, complete los campos obligatorios.", "Importante", MessageBoxButton.OK);
                if (_focusOnError)
                {
                    DependencyObject obj = Validator.GetFirstChildWithError(this);
                    if (obj != null && obj is UIElement)
                    {
                        Dispatcher.BeginInvoke(new Update(()=> ((UIElement)obj).Focus()));
                    }
                }
            }
            if (ret && ViewModel != null)
                return ViewModel.CanAccept(parameter);

            return ret;
        }

        public virtual bool CanCancel(object parameter)
        {
            if (ViewModel != null)
                return ViewModel.CanCancel(parameter);

            return true;
        }

        public virtual bool ShowDialog()
        {
            bool ret = false;

            Terminal.Instance.CurrentClient.ShowDialog(this,o => ret = o);

            return ret;
        }

        protected virtual void OnControlModeChanged(ControlMode newMode)
        {
            
        }

        public void GoHomeOrDisable()
        {
            if (!ZeroCommonClasses.Terminal.Instance.Session.Actions.Exists(ZeroBusiness.Actions.AppHome)
                || !ZeroCommonClasses.Terminal.Instance.Session.Actions[ZeroBusiness.Actions.AppHome].TryExecute())
            {
                IsEnabled = false;
            }
        }

        #region Implementation of IValidable

        public bool Validate()
        {
            return Validator.IsValid(this, TextBox.TextProperty);
        }

        #endregion
    }
}
