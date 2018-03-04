using EchoDesertTrips.Client.Entities;
using System;

namespace EchoDesertTrips.Desktop.Support
{
    public class AuthenticationEventArgs : EventArgs
    {
        public AuthenticationEventArgs(Operator op)
        {
            Operator = op;
        }
        public Operator Operator { get; set; }
    }
}
