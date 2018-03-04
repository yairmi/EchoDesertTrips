using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class Optional
    {
        public int OptionalId { get; set; }
        public string OptionalDescription { get; set; }
        public float PricePerPerson { get; set; }
        public float PriceInclusive { get; set; }
        public bool Visible { get; set; }
    }
}
