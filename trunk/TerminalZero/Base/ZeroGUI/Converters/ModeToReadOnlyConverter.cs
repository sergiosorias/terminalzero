using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI.Converters
{
    public class ModeToReadOnlyConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ControlMode controlMode = (ControlMode)value;
            return controlMode == ControlMode.ReadOnly;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
