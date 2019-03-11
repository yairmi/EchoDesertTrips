using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Client.Proxies.Service_Proxies;
using static Core.Common.Core.Const;

namespace Core.Common.UI.Core
{
    public class ClientSingle
    {
        private static readonly ClientSingle INSTANCE = new ClientSingle();
        private ClientSingle() { }
        public static ClientSingle Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        public BroadcastorServiceClient client;

        public void NotifyServer(string message, eMsgTypes msgType, Operator currentOperator)
        {
            client.NotifyServer(new EventDataType()
            {
                ClientName = currentOperator.OperatorName + "-" + currentOperator.OperatorId,
                EventMessage = message,
                MessageType = msgType
            });
        }
    }
}
