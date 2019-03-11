using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.UI.CustomEventArgs
{
    public class CustomerOrderEventArgs : EventArgs
    {
        public CustomerOrderEventArgs(Customer customer, Reservation reservation, bool isNew)
        {
            Customer = customer;
            Reservation = reservation;
            IsNew = isNew;
        }

        public Customer Customer { get; set; }
        public Reservation Reservation { get; set; }
        public bool IsNew { get; set; }
    }
}
