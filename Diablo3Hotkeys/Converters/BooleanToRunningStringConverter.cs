using System;
using System.Globalization;
using System.Windows.Data;

namespace DiabloIIIHotkeys.Converters
{
    internal class BooleanToRunningStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertedValue = false;

            try
            {
                if (value != null)
                {
                    convertedValue = (bool)value;
                }
            }
            catch
            {
            }

            return convertedValue ? "Running" : "Stopped";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
