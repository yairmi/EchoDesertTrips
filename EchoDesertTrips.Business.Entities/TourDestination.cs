using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class TourDestination : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourDestinationId { get; set; }
        [DataMember]
        public string TourDestinationName { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        public int EntityId
        {
            get
            {
                return TourDestinationId;
            }

            set
            {
                TourDestinationId = value;
            }
        }
    }
}
