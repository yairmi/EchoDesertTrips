using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class TourType
    {
        public int TourTypeId { get; set; }
        public string TourTypeName { get; set; }
        public float PricePerChild { get; set; }
        public float PricePerAdult { get; set; }
        public TourDestination TourDestination { get; set; }
        public int TourDestinationId { get; set; }
        public bool Private { get; set; }
        public string TourDescription { get; set; }
        public byte Days { get; set; }
        public bool Visible { get; set; }
    }
}
