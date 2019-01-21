using Core.Common.ServiceModel;
using EchoDesertTrips.Business.Entities;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Contracts
{
    [DataContract]
    public class InventoryData : DataContractBase
    {
        [DataMember]
        public TourType[] TourTypes { get; set; }
        [DataMember]
        public Hotel[] Hotels { get; set; }
        [DataMember]
        public Agency[] Agencies { get; set; }
        [DataMember]
        public Optional[] Optionals { get; set; }
        [DataMember]
        public RoomType[] RoomTypes { get; set; }
        [DataMember]
        public HotelRoomType[] HotelRoomTypes { get; set; }
        [DataMember]
        public Operator[] Operators { get; set; }
    }
}
