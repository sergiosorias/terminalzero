using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using ZeroCommonClasses.Helpers;

namespace ZeroGUI.Converters
{
    public class TypeToVisibilityConverter : IValueConverter
    {
        private static TypeToVisibilityConverter _Instance;
        public static TypeToVisibilityConverter Instance
        {
            get { return _Instance ?? (_Instance = new TypeToVisibilityConverter()); }
        }
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null || parameter == null)
                return Visibility.Collapsed;

            
            return  ComparisonExtentions.ContainsType(value.GetType(),Type.GetType(parameter.ToString())) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
