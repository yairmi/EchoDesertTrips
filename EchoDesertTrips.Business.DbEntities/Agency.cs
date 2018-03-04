using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class Agency
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string AgencyAddress { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public List<Agent> Agents { get; set; }
    }
}
