using EchoDesertTrips.Client.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    public class BroadcastorServiceClient : System.ServiceModel.DuplexClientBase<IBroadcastorService>, IBroadcastorService
    {
        public BroadcastorServiceClient(System.ServiceModel.InstanceContext callbackInstance) :
        base(callbackInstance)
        {
        }

        public void NotifyServer(EventDataType eventData)
        {
            Channel.NotifyServer(eventData);
        }

        public void RegisterClient(string clientName)
        {
            Channel.RegisterClient(clientName);
        }

        public void UnRegisterClient(string clientName)
        {
            Channel.UnRegisterClient(clientName);
        }
    }
}
