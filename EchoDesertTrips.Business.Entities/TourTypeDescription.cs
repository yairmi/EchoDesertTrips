using Core.Common.Contracts;
using Core.Common.Core;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class TourTypeDescription : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourTypeDescriptionId { get; set; }
        [DataMember]
        [MaxLength(500)]
        public string Description { get; set; }

        public int EntityId
        {
            get
            {
                return TourTypeDescriptionId;
            }

            set
            {
                TourTypeDescriptionId = value;
            }
        }
    }
}
