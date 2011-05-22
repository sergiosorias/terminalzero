using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ZeroGUI.Classes
{
    public static class Validator
    {
        public static bool IsValid(DependencyObject parent, params DependencyProperty[] properties)
        {
            foreach (DependencyProperty depProperty in properties)
            {
                BindingExpression be =
                  BindingOperations.GetBindingExpression(parent, depProperty);
                if (be != null) be.UpdateSource();
            }
            
            if (Validation.GetHasError(parent))
                return false;
            
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child, properties))
                {
                    return false;
                }
            } 
            
            return true;
        }

        public static DependencyObject GetFirstChildWithError(DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return parent;
            // Validate all the bindings on the children    
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child))
                {
                    return child;
                }
            }

            return null;
        }

        public static void UpdateBindingSources(DependencyObject obj,
                          params DependencyProperty[] properties)
        {
            foreach (DependencyProperty depProperty in properties)
            {
                //check whether the submitted object provides a bound property
                //that matches the property parameters
                BindingExpression be =
                  BindingOperations.GetBindingExpression(obj, depProperty);
                if (be != null) be.UpdateSource();
            }

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                //process child items recursively
                DependencyObject childObject = VisualTreeHelper.GetChild(obj, i);
                UpdateBindingSources(childObject, properties);
            }
        }
    }
}
