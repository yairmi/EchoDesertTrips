using Core.Common.ServiceModel;
using EchoDesertTrips.Business.Entities;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Contracts
{
    [DataContract]
    public class ReservationData : DataContractBase
    {
        [DataMember(EmitDefaultValue = false)]
        public Reservation DbReservation { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Reservation ClientReservation { get; set; }
        [DataMember]
        public bool InEdit { get; set; }
    }
}
