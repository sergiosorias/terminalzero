using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Data;

namespace ZeroGUI.Converters
{
    public class StringToResourceConverter : IValueConverter
    {
        private StringToResourceConverter()
        {
        }

        private static StringToResourceConverter _Instance;
        public static StringToResourceConverter Instance
        {
            get { return _Instance ?? (_Instance = new StringToResourceConverter()); }
        }


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "Not implemented method";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
