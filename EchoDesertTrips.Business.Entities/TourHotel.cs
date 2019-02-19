using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class TourHotel : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourHotelId { get; set; }
        [DataMember]
        virtual public Hotel Hotel { get; set; }
        [DataMember]
        public int? HotelId { get; set; }
        [DataMember]
        virtual public List<TourHotelRoomType> TourHotelRoomTypes { get; set; }
        [DataMember]
        public DateTime HotelStartDay { get; set; }
        public int EntityId
        {
            get
            {
                return TourHotelId;
            }

            set
            {
                TourHotelId = value;
            }
        }
    }
}
