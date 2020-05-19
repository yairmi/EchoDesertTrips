using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class CustomerEventArgs : EventArgs
    {
        public CustomerEventArgs(Customer customer, bool isNew)
        {
            Customer = customer;
            IsNew = isNew;
        }

        public Customer Customer { get; private set; }
        public bool IsNew { get; private set; }
    }
}
