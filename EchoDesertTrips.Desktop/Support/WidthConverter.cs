using System;
using System.Globalization;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.Support
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.SystemParameters.PrimaryScreenWidth * System.Convert.ToInt32(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
