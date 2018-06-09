using EchoDesertTrips.Business.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BroadcastorManager : ManagerBase, IBroadcastorService 
    {
        // list of all currently connected active clients
        private static Dictionary<string, IBroadcastorCallBack> clients = new Dictionary<string, IBroadcastorCallBack>();
        //synchronize the access to the variable clients among multiple threads
        private static object locker = new object();

        public void RegisterClient(string clientName)
        {
            ExecuteFaultHandledOperation(() =>
            {
                if (!string.IsNullOrEmpty(clientName))
                {
                    var callback =
                        OperationContext.Current.GetCallbackChannel<IBroadcastorCallBack>();
                    lock (locker)
                    {
                        //remove the old client
                        if (clients.Keys.Contains(clientName))
                            clients.Remove(clientName);
                        clients.Add(clientName, callback);
                    }
                }
            });
        }

        public void UnRegisterClient(string clientName)
        {
            ExecuteFaultHandledOperation(() =>
            {
                lock (locker)
                {
                    if (clients.Keys.Contains(clientName))
                        clients.Remove(clientName);
                }
            });
        }

        public void NotifyServer(EventDataType eventData)
        {
            ExecuteFaultHandledOperation(() =>
            {
                lock (locker)
                {
                    var inactiveClients = new List<string>();
                    foreach (var client in clients)
                    {
                        if (client.Key != eventData.ClientName)
                        {
                            try
                            {
                                client.Value.BroadcastToClient(eventData);
                            }
                            catch (Exception ex)
                            {
                                inactiveClients.Add(client.Key);
                            }
                        }
                    }

                    if (inactiveClients.Count > 0)
                    {
                        foreach (var client in inactiveClients)
                        {
                            clients.Remove(client);
                        }
                    }
                }
            });
        }
    }
}
