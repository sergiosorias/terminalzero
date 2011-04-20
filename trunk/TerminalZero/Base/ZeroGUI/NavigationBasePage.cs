using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI
{
    public class NavigationBasePage : UserControl
    {
        public ControlMode ControlMode
        {
            get { return (ControlMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); OnControlModeChanged(value); }
        }

        // Using a DependencyProperty as the backing store for ControlMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("ControlMode", typeof(ControlMode), typeof(NavigationBasePage), null);

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            ControlMode = ControlMode.New;
        }

        public virtual bool CanAccept(object parameter)
        {
            return true;
        }

        public virtual bool CanCancel(object parameter)
        {
            return true;
        }

        protected virtual void OnControlModeChanged(ControlMode newMode)
        {
            
        }

        protected void GoHomeOrDisable(ITerminal terminal)
        {
            ZeroAction action;
            if (!terminal.Manager.ExistsAction(ZeroBusiness.Actions.AppHome, out action)
                || !terminal.Manager.ExecuteAction(action))
            {
                IsEnabled = false;
            }
        }
    }
}
