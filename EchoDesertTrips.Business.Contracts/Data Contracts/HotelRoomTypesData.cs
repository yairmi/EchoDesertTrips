using Core.Common.ServiceModel;
using EchoDesertTrips.Business.Entities;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Contracts
{
    [DataContract]
    public class HotelRoomTypesData : DataContractBase
    {
        [DataMember]
        public Hotel Hotel { get; set; }
        [DataMember]
        public RoomType RoomType { get; set; }
        [DataMember]
        public float PricePerChild { get; set; }
        [DataMember]
        public float PricePerAdult { get; set; }
    }
}
