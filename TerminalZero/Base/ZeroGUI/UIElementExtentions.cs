using System.Windows;
using System.Windows.Input;

namespace ZeroGUI
{
    public static class UIElementExtentions
    {
        public static bool GetTabOnEnter(DependencyObject obj)
        {
            return (bool)obj.GetValue(TabOnEnterProperty);
        }

        public static void SetTabOnEnter(DependencyObject obj, bool value)
        {
            obj.SetValue(TabOnEnterProperty, value);
        }

        // Using a DependencyProperty as the backing store for TabOnEnter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabOnEnterProperty =
            DependencyProperty.RegisterAttached("TabOnEnter", typeof(bool), typeof(UIElementExtentions), new UIPropertyMetadata(false, OnTabOnEnterChanged));

        private static void OnTabOnEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is UIElement)
            {
                ((UIElement)d).KeyDown += (o, args) =>
                {
                    var uie = args.OriginalSource as UIElement;
                    if (args.Key == Key.Enter)
                    {
                        args.Handled = true;
                        uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                };
            }
        }

        
        
    }
}
