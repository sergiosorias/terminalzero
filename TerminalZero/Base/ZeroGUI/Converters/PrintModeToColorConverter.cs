using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI.Converters
{
    public class PrintModeToColorConverter : IValueConverter
    {
        private static PrintModeToColorConverter _instance;

        public static PrintModeToColorConverter Instance
        {
            get
            {
                return _instance ?? (_instance = new PrintModeToColorConverter());
            }
        }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? printMode = value as int?;
            Color result = Colors.Transparent;
            if (printMode.HasValue && printMode.Value == (int)PrintMode.UseTax)
                result = Colors.Red;

            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
