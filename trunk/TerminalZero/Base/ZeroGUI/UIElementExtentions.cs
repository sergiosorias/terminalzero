using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            if (d is UIElement)
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


        public static string GetListCommanderName(DependencyObject obj)
        {
            return (string)obj.GetValue(ListCommanderNameProperty);
        }

        public static void SetListCommanderName(DependencyObject obj, string value)
        {
            obj.SetValue(ListCommanderNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for ListCommanderName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListCommanderNameProperty =
            DependencyProperty.RegisterAttached("ListCommanderName", typeof(string), typeof(UIElementExtentions), new UIPropertyMetadata(null, OnListCommanderNameChanged));

        private static void OnListCommanderNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d!= null && d is DataGrid)
            {
                var list = d as DataGrid;
                list.Loaded += (hh, hj) =>
                {
                    Window activeWindow = null;
                    for (int i = 0; i < Application.Current.Windows.Count; i++)
                    {
                        if (Application.Current.Windows[i].IsActive)
                        {
                            activeWindow = Application.Current.Windows[i];
                            break;
                        }
                    }
                    if (activeWindow != null)
                    {
                        var element = FindVisualChildByName<FrameworkElement>(activeWindow, e.NewValue.ToString());
                        if (element != null)
                        {
                            SetTabOnEnter(element, true);
                            (element).PreviewKeyDown += (o, args) =>
                                                   {
                                                       switch (args.Key)
                                                       {
                                                           case Key.Up:
                                                               if (list.SelectedIndex > 0)
                                                                   list.SelectedIndex--;
                                                               args.Handled = true;
                                                               break;
                                                           case Key.Down:
                                                               if (list.SelectedIndex <= list.Items.Count)
                                                               {
                                                                   list.SelectedIndex++;
                                                               }
                                                               args.Handled = true;
                                                               break;
                                                       }
                                                   };

                        }
                    }
                };
            }
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {      // Confirm parent and childName are valid.   
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child    
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree      
                    foundChild = FindChild<T>(child, childName);
                    // If the child is found, break so we do not overwrite the found child.       
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search      
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name        
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.      
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

    }
}
