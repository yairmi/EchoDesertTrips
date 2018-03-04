using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class TourOptional
    {
        public int TourId { get; set; }
        public int OptionalId { get; set; }
        public Optional Optional { get; set; }
    }
}
