using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class Nationality
    {
        public int NationalityId { get; set; }
        public string NationalityName { get; set; }
        public bool Visible { get; set; }
    }
}
