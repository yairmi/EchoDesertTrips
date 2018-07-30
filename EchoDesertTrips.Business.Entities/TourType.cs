using Core.Common.Contracts;
using Core.Common.Core;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class TourType : EntityBase, IIdentifiableEntity
    {
        /*public TripType()
        {
            Trips = new List<Trip>();
        }*/
        [DataMember]
        public int TourTypeId { get; set; }
        [DataMember]
        public string TourTypeName { get; set; }
        [DataMember]
        public string AdultPrices { get; set; }
        [DataMember]
        public string ChildPrices { get; set; }
        [DataMember]
        public string InfantPrices { get; set; }
        [DataMember]
        public string Destinations { get; set; }
        [DataMember]
        public bool Private { get; set; }
        [DataMember]
        public byte Days { get; set; }
        [DataMember]
        virtual public List<TourTypeDescription> TourTypeDescriptions { get; set; }
        [DataMember]
        public bool IncramentExternalId { get; set; }
        [DataMember]
        public bool Visible { get; set; }
        public int EntityId
        {
            get
            {
                return TourTypeId;
            }

            set
            {
                TourTypeId = value;
            }
        }
    }
}
