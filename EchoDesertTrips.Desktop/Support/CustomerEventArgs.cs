using EchoDesertTrips.Client.Entities;
using System;

namespace EchoDesertTrips.Desktop.Support
{
    public class CustomerEventArgs : EventArgs
    {
        //Remove CustomerWrapper
        public CustomerEventArgs(CustomerWrapper customer, bool isNew)
        {
            Customer = customer;
            IsNew = isNew;
        }
        //Remove CustomerWrapper
        public CustomerWrapper Customer { get; set; }
        public bool IsNew { get; set; }
    }
}
