using Core.Common.Contracts;
using Core.Common.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract]
    public class TourType : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourTypeId { get; set; }
        [DataMember]
        [Required]
        [MaxLength(100)]
        public string TourTypeName { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        public string AdultPrices { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        public string ChildPrices { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(300)]
        public string InfantPrices { get; set; }
        [DataMember]
        [MaxLength(300)]
        public string Destinations { get; set; }
        [DataMember]
        public bool Private { get; set; }
        [DataMember]
        public byte Days { get; set; }
        [DataMember]
        public List<TourTypeDescription> TourTypeDescriptions { get; set; }
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
