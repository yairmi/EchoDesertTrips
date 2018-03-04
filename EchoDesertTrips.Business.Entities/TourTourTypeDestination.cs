using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class TourTourTypeDestination : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourId { get; set; }
        [DataMember]
        public int TourTypeDestinationId { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        public int EntityId { get; set; }
    }
}
