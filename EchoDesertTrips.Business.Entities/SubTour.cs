﻿using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class SubTour : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int SubTourId { get; set; }
        [DataMember]
        public string DestinationName { get; set; }
        //[DataMember]
        //public bool Private { get; set; }
        //[DataMember]
        //public DateTime StartDate { get; set; }
        public int EntityId
        {
            get
            {
                return SubTourId;
            }

            set
            {
                SubTourId = value;
            }
        }
    }
}
