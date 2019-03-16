using EchoDesertTrips.Client.Entities;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Core.Common.UI.PubSubEvent
{
    public class TourEditedEvent : PubSubEvent<Tour>
    {
    }
}
