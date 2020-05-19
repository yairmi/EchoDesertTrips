using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class HotelRoomTypeDaysRange : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int HotelRoomTypeDaysRangeId { get; set; }
        [DataMember]
        public DateTime StartDaysRange { get; set; }
        [DataMember]
        public DateTime EndDaysRange { get; set; }
        [DataMember]
        public decimal PricePerPerson { get; set; }
        public int EntityId
        {
            get
            {
                return HotelRoomTypeDaysRangeId;
            }
            set
            {
                HotelRoomTypeDaysRangeId = value;
            }
        }
    }
}
