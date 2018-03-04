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
    public class OperatorTrip : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int OperatorTripId { get; set; }
        [DataMember]
        public int OperatorId { get; set; }
        [DataMember]
        public int TripId { get; set; }

        public int EntityId
        {
            get
            {
                return OperatorTripId;
            }

            set
            {
                OperatorTripId = value;
            }
        }
    }
}
