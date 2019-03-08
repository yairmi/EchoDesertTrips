using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Operator : EntityBase, IIdentifiableEntity, ICloneable
    {
        [DataMember]
        public int OperatorId { get; set; }
        [DataMember]
        public string OperatorName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string OperatorFullName { get; set; }
        [DataMember]
        public bool Admin { get; set; }

        public int EntityId
        {
            get
            {
                return OperatorId;
            }

            set
            {
                OperatorId = value;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
