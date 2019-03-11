using Core.Common.UI.CustomEventArgs;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Core.Common.UI.PubSubEvent
{
    public class ReservationCancelledEvent : PubSubEvent<ReservationEventArgs>
    {
    }
}
