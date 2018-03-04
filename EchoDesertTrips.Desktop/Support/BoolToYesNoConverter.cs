using System;
using System.Windows.Data;
using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.Support
{
    public class BoolToYesNoConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = value as Customer;
            if (item != null)
            return (item.HasVisa) ? "Yes" : "No";
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
