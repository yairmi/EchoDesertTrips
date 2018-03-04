using EchoDesertTrips.Client.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    public class BroadcastorCallback : IBroadcastorCallBack
    {
        private System.Threading.SynchronizationContext syncContext =
            AsyncOperationManager.SynchronizationContext;

        private EventHandler<EventDataType> _broadcastorCallBackHandler;
        public void SetHandler(EventHandler<EventDataType> handler)
        {
            this._broadcastorCallBackHandler = handler;
        }

        public void BroadcastToClient(EventDataType eventData)
        {
            syncContext.Post(new System.Threading.SendOrPostCallback(OnBroadcast),
                  eventData);
        }

        private void OnBroadcast(object eventData)
        {
            this._broadcastorCallBackHandler.Invoke(eventData, (EventDataType)eventData);
        }
    }
}
