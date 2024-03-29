﻿using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class TourOptional : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourId { get; set; }
        [DataMember]
        public int OptionalId { get; set; }
        [DataMember]
        public bool PriceInclusive { get; set; }
        [DataMember]
        virtual public Optional Optional { get; set; }
        [DataMember]
        public decimal PricePerPerson { get; set; }
        [DataMember]
        public decimal PriceInclusiveValue { get; set; }
        public int EntityId { get; set; }
    }
}
