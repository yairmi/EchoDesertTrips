using Core.Common.Contracts;
using Core.Common.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class Hotel : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int HotelId { get; set; }
        [DataMember]
        public string HotelName { get; set; }
        [DataMember]
        public string HotelAddress { get; set; }
        [DataMember]
        public List<HotelRoomType> HotelRoomTypes { get; set; }
        [DataMember]
        public bool Visible { get; set; }


        public int EntityId
        {
            get
            {
                return HotelId;
            }

            set
            {
                HotelId = value;
            }
        }
    }
}
