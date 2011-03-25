using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI
{
    public class ZeroBasePage : UserControl
    {
        public ZeroBasePage()
        {
            ControlMode = ControlMode.New;
        }

        public ControlMode ControlMode
        {
            get { return (ControlMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ControlMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("ControlMode", typeof(ControlMode), typeof(ZeroBasePage), null);
        
        public virtual bool CanAccept(object parameter)
        {
            return true;
        }

        public virtual bool CanCancel(object parameter)
        {
            return true;
        }

        protected virtual void OnModeChanged()
        {
            
        }

            
    }
}
