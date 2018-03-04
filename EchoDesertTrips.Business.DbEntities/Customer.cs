using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.DbEntities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string IdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public DateTime DateOfBirdth { get; set; }
        public string PassportNumber { get; set; }
        public DateTime IssueData { get; set; }
        public DateTime ExpireyDate { get; set; }
        public Nationality Nationality { get; set; }
        public int NationalityId { get; set; }
        public bool HasVisa { get; set; }
        public string LoginEmail { get; set; }
    }
}
