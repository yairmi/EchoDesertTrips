using Core.Common.Contracts;
using Core.Common.Core;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class TourTypeDestination : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        [Key]
        public int TourTypeDestinationId { get; set; }
        [DataMember]
        public int TourTypeId { get; set; }
        [DataMember]
        public int TourDestinationId { get; set; }
        [DataMember]
        public TourDestination TourDestination {get;set;}
        public int EntityId
        { get; set; }
    }
}
