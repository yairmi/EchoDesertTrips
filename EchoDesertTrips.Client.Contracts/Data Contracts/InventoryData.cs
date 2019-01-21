using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Contracts
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
