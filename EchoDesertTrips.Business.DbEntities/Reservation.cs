using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public Operator Operator { get; set; }
        public int? OperatorId { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Tour> Tours { get; set; }
        public Agency Agency { get; set; }
        public int? AgencyId { get; set; }
        public Agent Agent { get; set; }
        public int? AgentId { get; set; }
        public decimal AdvancePayment { get; set; }
        public DateTime PickUpTime { get; set; }
        public string Comments { get; set; }
        public string Messages { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime UpdateTime { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
