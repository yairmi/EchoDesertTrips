using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class Operator
    {
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public string Password { get; set; }
        public bool Admin { get; set; }
    }
}
