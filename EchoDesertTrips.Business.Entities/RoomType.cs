using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class RoomType : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int RoomTypeId { get; set; }
        [DataMember]
        public string RoomTypeName { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        public int EntityId
        {
            get
            {
                return RoomTypeId;
            }

            set
            {
                RoomTypeId = value;
            }
        }
    }
}
