using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI.Converters
{

    public class ControlModeToVisibilityConverter : IValueConverter
    {
        private static ControlModeToVisibilityConverter _instance;
        public static ControlModeToVisibilityConverter Instance
        {
            get { return _instance ?? (_instance = new ControlModeToVisibilityConverter()); }
        }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ControlMode mode;
            if (Enum.TryParse(parameter.ToString(), out mode))
            {
                var modeToBind = (ControlMode)value;
                return modeToBind.HasFlag(mode) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
