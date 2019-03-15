using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class OptionalEventArgs : EventArgs
    {
        public OptionalEventArgs(Optional optional, bool isNew)
        {
            Optional = optional;
            bIsNew = isNew;
        }
        public Optional Optional { get; set; }
        public bool bIsNew { get; set; }
    }
}
