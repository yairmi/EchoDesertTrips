using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class HotelRoomType : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        [Key]
        public int HotelRoomTypeId { get; set; }
        //[DataMember]
        //public Hotel Hotel { get; set; }
        [DataMember]
        public int HotelId { get; set; }
        [DataMember]
        public int RoomTypeId { get; set; }
        [DataMember]
        public RoomType RoomType { get; set; }
        [DataMember]
        public float PricePerPerson { get; set; }
        [DataMember]
        [NotMapped]
        public string HotelName { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        public int EntityId
        {
            get
            {
                return HotelRoomTypeId;
            }
            set
            {
                HotelRoomTypeId = value;
            }
        }
    }
}
