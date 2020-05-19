using Core.Common.Contracts;
using Core.Common.Core;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class OptionalPrices : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int OptionalPricesId { get; set; }

        [DataMember]
        public decimal PricePerPerson { get; set; }

        [DataMember]
        public decimal PriceInclusive { get; set; }

        public int EntityId
        {
            get
            {
                return OptionalPricesId;
            }

            set
            {
                OptionalPricesId = value;
            }
        }
    }
}
