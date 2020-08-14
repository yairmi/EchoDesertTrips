using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Client.Proxies.Service_Proxies;
using System;
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

        private OperatorSingle CurrentOperator
        {
            get
            {
                return OperatorSingle.Instance;
            }
        }

        public bool Registered { get; set; }

        public BroadcastorServiceClient client;

        public void NotifyServer(string message, eMsgTypes msgType)
        {
            client.NotifyServer(new EventDataType()
            {
                ClientName = CurrentOperator.Operator.OperatorName + "-" + CurrentOperator.Operator.OperatorId,
                EventMessage = message,
                MessageType = msgType
            });
        }

        public bool RegisterClient(EventHandler<BroadcastMessage> HandleBroadcast)
        {
            if (client != null)
            {
                client.Abort();
                client = null;
            }

            var cb = new BroadcastorCallback();
            cb.SetHandler(HandleBroadcast);

            System.ServiceModel.InstanceContext context =
                new System.ServiceModel.InstanceContext(cb);
            client = new BroadcastorServiceClient(context);

            var operatorNameId = $"{CurrentOperator.Operator.OperatorName}-{CurrentOperator.Operator.OperatorId}";
            Registered = client.RegisterClient(operatorNameId);
            return Registered;
        }

        public bool UnRegisterClient()
        {
            if (CurrentOperator.Operator == null)
                return true;

            bool bSucceeded = false;
            var operatorNameId = $"{CurrentOperator.Operator.OperatorName}-{CurrentOperator.Operator.OperatorId}";

            if (client == null)
            {
                CreateClient();
                log.Debug("Client was NULL. After Client Creation");
            }
            else if (client != null && client.InnerDuplexChannel.State == System.ServiceModel.CommunicationState.Faulted)
            {
                client.Abort();
                client = null;
                CreateClient();
                log.Debug("Client was NOT NULL. After Client Creation");
            }

            bSucceeded = client.UnRegisterClient(operatorNameId);

            return bSucceeded;
        }

        private void CreateClient()
        {
            //var cb = new BroadcastorCallback();
            //cb.SetHandler(HandleBroadcast);

            var context =
                new System.ServiceModel.InstanceContext(new BroadcastorCallback());
            client = new BroadcastorServiceClient(context);
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
