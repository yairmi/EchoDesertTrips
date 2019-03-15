using Core.Common.UI.CustomEventArgs;
using EchoDesertTrips.Client.Entities;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Core.Common.UI.PubSubEvent
{
    public class RoomTypeUpdatedEvent : PubSubEvent<RoomTypeEventArgs>
    {
    }
}
