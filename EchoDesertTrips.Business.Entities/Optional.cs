using Core.Common.Contracts;
using Core.Common.Core;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class Optional : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int OptionalId { get; set; }

        [DataMember]
        [Required]
        [MaxLength(50)]
        public string OptionalDescription { get; set; }

        [DataMember]
        public float PricePerPerson { get; set; } 

        [DataMember]
        public float PriceInclusive { get; set; }

        [DataMember]
        public bool Visible { get; set; }

        public int EntityId
        {
            get
            {
                return OptionalId;
            }

            set
            {
                OptionalId = value;
            }
        }
    }
}
