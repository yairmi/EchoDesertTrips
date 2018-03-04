using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Entities;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Client.Contracts
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
