using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.UI.Core
{
    public class ReservationsSingle
    {
        private static readonly ReservationsSingle INSTANCE = new ReservationsSingle();
        private ReservationsSingle() { }
        public static ReservationsSingle Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        public RangeObservableCollection<Reservation> reservations;
        public RangeObservableCollection<Reservation> continualReservations;

        public void RemoveReservation(int reservationId)
        {
            var reservation = reservations.FirstOrDefault(item => item.ReservationId == reservationId);
            if (reservation != null)
            {
                reservations.Remove(reservation);

            }
        }
    }
}
