using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class TourDestination
    {
        public int TourDestinationId { get; set; }
        public string TourDestinationName { get; set; }
        public bool Visible { get; set; }
    }
}
