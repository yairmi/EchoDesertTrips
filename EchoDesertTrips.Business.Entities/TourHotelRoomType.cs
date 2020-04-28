using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class TourHotelRoomType : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourHotelRoomTypeId { get; set; }
        [DataMember]
        public int HotelRoomTypeId { get; set; }
        [DataMember]
        virtual public HotelRoomType HotelRoomType { get; set; }
        [DataMember]
        public int Capacity { get; set; }
        [DataMember]
        public int Persons { get; set; }
        [DataMember]
        public float PricePerPerson { get; set; } 
        public int EntityId
        {
            get
            {
                return TourHotelRoomTypeId;
            }
            set
            {
                TourHotelRoomTypeId = value;
            }
        }
    }
}
