using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.Support
{
    public class ViewModelAuthenticatedToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value == false ? Visibility.Collapsed : Visibility.Visible);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
