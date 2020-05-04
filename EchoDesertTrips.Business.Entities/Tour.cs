using Core.Common.Contracts;
using Core.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Business.Entities
{
    [DataContract(IsReference = true)]
    public class Tour : EntityBase, IIdentifiableEntity
    {
        [DataMember]
        public int TourId { get; set; }
        [DataMember]
        public TourType TourType { get; set; }
        [DataMember]
        public int TourTypeId { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        [Required]
        [MaxLength(100)]
        public string PickupAddress { get; set; }
        [DataMember]
        public List<TourOptional> TourOptionals { get; set; }
        [DataMember]
        public List<TourHotel> TourHotels { get; set; }
        [DataMember]
        public List<SubTour> SubTours { get; set; }
        [DataMember]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string TourTypePrice { get; set; }
        public int EntityId
        {
            get
            {
                return TourId;
            }

            set
            {
                TourId = value;
            }
        }
    }
}
