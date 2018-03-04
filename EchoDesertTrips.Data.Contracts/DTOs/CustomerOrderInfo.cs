using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Data.Contracts.DTOs
{
    public class CustomerOrderInfo
    {
        public Customer customer { get; set; }
        public Reservation reservation { get; set; }
        public Tour trip { get; set; }
        public Agency agency { get; set; }
        public Agent agent { get; set; }
    }
}
