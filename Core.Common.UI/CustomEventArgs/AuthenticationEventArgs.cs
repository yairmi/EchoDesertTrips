using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class AuthenticationEventArgs : EventArgs
    {
        public AuthenticationEventArgs(Operator op)
        {
            Operator = op;
        }
        public Operator Operator { get; private set; }
    }
}
