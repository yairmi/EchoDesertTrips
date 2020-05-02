using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Operator : EntityBase, IIdentifiableEntity, ICloneable
    {
        [DataMember]
        public int OperatorId { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string OperatorName { get; set; }
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
        [DataMember]
        [MaxLength(100)]
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
