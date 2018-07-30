using System;
using System.Globalization;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.Support
{
    public class DateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = @"dd/MM/yyyy";
            
            var Date = DateTime.ParseExact(value.ToString(), format, CultureInfo.InvariantCulture);
            return Date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
