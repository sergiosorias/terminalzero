﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ZeroGUI.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        private static NullToVisibilityConverter _instance;
        public static NullToVisibilityConverter Instance
        {
            get { return _instance ?? (_instance = new NullToVisibilityConverter()); }
        }

        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
