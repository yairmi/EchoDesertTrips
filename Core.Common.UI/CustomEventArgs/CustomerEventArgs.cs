﻿using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class CustomerEventArgs : EventArgs
    {
        //Remove CustomerWrapper
        public CustomerEventArgs(Customer customer, bool isNew)
        {
            Customer = customer;
            IsNew = isNew;
        }
        //Remove CustomerWrapper
        public Customer Customer { get; set; }
        public bool IsNew { get; set; }
    }
}