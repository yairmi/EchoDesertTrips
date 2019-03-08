using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Entities
{
    public class CustomerForReport
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirdth { get; set; }
        public string Passport { get; set; }
        public string IssueDate { get; set; }
        public string Expiry { get; set; }
        public string Nationality { get; set; }
        public string HasVisa { get; set; }
        public double AgeInDays { get; set; }
    }
}
