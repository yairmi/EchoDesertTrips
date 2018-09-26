using System;
using System.Globalization;
using System.Windows.Data;
using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.Support
{
    public class ReservationTotalCustomers : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as Reservation;
            if (item != null)
                return (item.Adults + item.Childs + item.Infants);
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
