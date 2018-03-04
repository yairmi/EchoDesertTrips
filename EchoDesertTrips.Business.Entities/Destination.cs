using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Destination : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int DestinationId { get; set; }
        [DataMember]
        public string DestinationName { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        public int EntityId
        {
            get
            {
                return DestinationId;
            }

            set
            {
                DestinationId = value;
            }
        }
    }
}
