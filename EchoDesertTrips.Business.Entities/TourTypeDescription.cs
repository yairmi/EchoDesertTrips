using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class TourTypeDescription : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourTypeDescriptionId { get; set; }
        [DataMember]
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
