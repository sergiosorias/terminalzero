using System;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroGUI.Classes;

namespace ZeroGUI
{
    public class NavigationBasePage : UserControl, IValidable
    {
        private bool _focusOnError = false;
        public bool FocusOnError
        {
            get { return _focusOnError; }
            set { _focusOnError = value; }
        }

        public ControlMode ControlMode
        {
            get { return (ControlMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); OnControlModeChanged(value); }
        }

        // Using a DependencyProperty as the backing store for ControlMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("ControlMode", typeof(ControlMode), typeof(NavigationBasePage), null);

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
                MessageBox.Show("Por favor, complete los campos obligatorios.", "Importante", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                if (_focusOnError)
                {
                    DependencyObject obj = Validator.GetFirstChildWithError(this);
                    if (obj != null && obj is UIElement)
                    {
                        Dispatcher.BeginInvoke(new Update(()=> ((UIElement)obj).Focus()));
                    }
                }
            }
            
            return ret;
        }

        public virtual bool CanCancel(object parameter)
        {
            return true;
        }

        protected virtual void OnControlModeChanged(ControlMode newMode)
        {
            
        }

        protected void GoHomeOrDisable()
        {
            ZeroAction action;
            if (!ZeroCommonClasses.Terminal.Instance.Manager.ExistsAction(ZeroBusiness.Actions.AppHome, out action)
                || !ZeroCommonClasses.Terminal.Instance.Manager.ExecuteAction(action))
            {
                IsEnabled = false;
            }
        }

        #region Implementation of IValidable

        public bool Validate()
        {
            return Validator.IsValid(this);
        }

        #endregion
    }
}
