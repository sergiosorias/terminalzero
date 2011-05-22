using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ZeroGUI.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        private StringFormatConverter()
        {
        }

        private static StringFormatConverter _Instance;
        public static StringFormatConverter Instance
        {
            get { return _Instance ?? (_Instance = new StringFormatConverter()); }
        }


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format(GetFormatFromParameter(parameter), value);
        }

        private string GetFormatFromParameter(object parameter)
        {
            return parameter == null ? "{0}" : parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
