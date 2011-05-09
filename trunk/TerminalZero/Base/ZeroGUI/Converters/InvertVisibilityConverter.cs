using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ZeroGUI.Converters
{
    public class InvertVisibilityConverter :IValueConverter
    {
        private static InvertVisibilityConverter _Instance;
        public static InvertVisibilityConverter Instance
        {
            get { return _Instance ?? new InvertVisibilityConverter(); }
        }
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if((Visibility)value == Visibility.Visible)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
