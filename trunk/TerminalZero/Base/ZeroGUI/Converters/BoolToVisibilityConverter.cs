using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ZeroGUI.Converters
{
    public class BoolToVisibilityConverter :IValueConverter
    {
        private static BoolToVisibilityConverter _Instance;
        public static BoolToVisibilityConverter Instance
        {
            get { return _Instance ?? new BoolToVisibilityConverter(); }
        }
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool) value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
