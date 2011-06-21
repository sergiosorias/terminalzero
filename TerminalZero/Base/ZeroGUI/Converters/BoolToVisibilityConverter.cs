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
        private static BoolToVisibilityConverter _instance;
        public static BoolToVisibilityConverter Instance
        {
            get { return _instance ??  (_instance =  new BoolToVisibilityConverter()); }
        }
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(parameter!=null)
            {
                bool param;
                if(bool.TryParse(parameter.ToString(),out param))
                    return ((bool)value) == param ? Visibility.Visible : Visibility.Collapsed;
            }
            return ((bool) value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }

        #endregion
    }
}
