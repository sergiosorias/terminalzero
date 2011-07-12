using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLFramework
{
    public static class ControlsExtentions
    {
        public static Point GetPosition(UIElement control, UIElement parent)
        {
            GeneralTransform gt = control.TransformToVisual(parent);
            return gt.Transform(new Point(0, 0));;
        }

        public static T FindAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T o = obj as T; if (o != null)
                    return o; obj = VisualTreeHelper.GetParent(obj);
            } return null;
        }
        public static T FindAncestor<T>(this UIElement obj) where T : UIElement
        {
            return FindAncestor<T>((DependencyObject)obj);
        }
    }
}
