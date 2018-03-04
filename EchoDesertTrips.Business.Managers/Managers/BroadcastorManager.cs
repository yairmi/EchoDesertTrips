using EchoDesertTrips.Business.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BroadcastorManager : IBroadcastorService
    {
        // list of all currently connected active clients
        private static Dictionary<string, IBroadcastorCallBack> clients = new Dictionary<string, IBroadcastorCallBack>();
        //synchronize the access to the variable clients among multiple threads
        private static object locker = new object();

        public void RegisterClient(string clientName)
        {
            if (clientName != null && clientName != "")
            {
                try
                {
                    IBroadcastorCallBack callback =
                        OperationContext.Current.GetCallbackChannel<IBroadcastorCallBack>();
                    lock (locker)
                    {
                        //remove the old client
                        if (clients.Keys.Contains(clientName))
                            clients.Remove(clientName);
                        clients.Add(clientName, callback);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void UnRegisterClient(string clientName)
        {
            try
            {
                lock (locker)
                {
                    if (clients.Keys.Contains(clientName))
                        clients.Remove(clientName);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void NotifyServer(EventDataType eventData)
        {
            lock (locker)
            {
                var inactiveClients = new List<string>();
                foreach (var client in clients)
                {
                    //if (client.Key != eventData.ClientName)
                    //{
                        try
                        {
                            client.Value.BroadcastToClient(eventData);
                        }
                        catch (Exception ex)
                        {
                            inactiveClients.Add(client.Key);
                        }
                    //}
                }

                if (inactiveClients.Count > 0)
                {
                    foreach (var client in inactiveClients)
                    {
                        clients.Remove(client);
                    }
                }
            }
        }
    }
}
