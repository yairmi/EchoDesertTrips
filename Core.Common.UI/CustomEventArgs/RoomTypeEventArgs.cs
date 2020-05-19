using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class RoomTypeEventArgs : EventArgs
    {
        public RoomTypeEventArgs(RoomType roomType, bool isNew)
        {
            RoomType = roomType;
            bIsNew = isNew;
        }
        public RoomType RoomType { get; private set; }
        public bool bIsNew { get; private set; }
    }
}
