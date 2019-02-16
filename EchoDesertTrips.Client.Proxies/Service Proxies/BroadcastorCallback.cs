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

        private EventHandler<BroadcastMessage> _broadcastorCallBackHandler;
        public void SetHandler(EventHandler<BroadcastMessage> handler)
        {
            this._broadcastorCallBackHandler = handler;
        }

        public void BroadcastToClient(BroadcastMessage Message)
        {
            syncContext.Post(new System.Threading.SendOrPostCallback(OnBroadcast),
                  Message);
        }

        private void OnBroadcast(object Message)
        {
            this._broadcastorCallBackHandler.Invoke(Message, (BroadcastMessage)Message);
        }
    }
}
