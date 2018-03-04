using System;
using System.Linq;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.Support
{
    public class RoomTypeIdToRoomTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //var RoomTypes = ((System.Windows.Data.CollectionViewGroup)(value))?.Items;
            //var customers = new ObservableCollection<Customer>();
            //foreach (var reservation in items.Cast<Reservation>())
            //{
            //    foreach (var customer in reservation.Customers)
            //    {
            //        customers.Add(customer);
            //    }
            //}
            //return customers;
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
