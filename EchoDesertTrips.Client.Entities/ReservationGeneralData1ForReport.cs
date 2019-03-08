using System;
using System.Collections.Generic;

namespace EchoDesertTrips.Client.Entities
{
    public class ReservationGeneralData1ForReport
    {
        public int ReservationId { get; set; }
        public int Pax { get; set; }
        public string HotelsStartDay { get; set; }
        public string RoomTypes { get; set; }
        public string OperatorName { get; set; }
        public string Specials { get; set; }
        public string FirstCustomerInReservation { get; set; }
        public string Hotels { get; set; }
    }
}
