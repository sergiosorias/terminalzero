using System;
using System.Globalization;
using System.Windows.Data;

namespace TerminalZeroWebClient.Classes
{
    public class DoubleFormatter : IValueConverter
    {
        // This converts the DateTime object to the string to display.
        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            // Retrieve the format string and use it to format the value.
            try
            {
                double d = double.Parse(value.ToString());
                return (int)d;
            }
            catch (Exception)
            {

            }
            return value;

            // If the format string is null or empty, simply call ToString()
            // on the value.

        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}