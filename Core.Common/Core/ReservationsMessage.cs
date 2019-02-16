using System;
using System.Collections.Generic;
using static Core.Common.Core.Const;

namespace Core.Common.Core
{
    public class ReservationsMessage
    {
        public List<ReservationMessage> ReservationsIds;
    }

    public class ReservationMessage
    {
        public ReservationMessage() { }
        public ReservationMessage(int reservationId, eOperation operation)
        {
            ReservationId = reservationId;
            Operation = operation;
        }
        public int ReservationId { get; set; }
        public eOperation Operation { get; set; }
    }
}
