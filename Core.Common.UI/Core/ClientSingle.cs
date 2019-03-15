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

        public void RegisterClient(EventHandler<BroadcastMessage> HandleBroadcast)
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

            var operatorNameId = CurrentOperator.Operator.OperatorName + "-" + CurrentOperator.Operator.OperatorId;

            client.RegisterClient(operatorNameId);
        }

        public void UnRegisterClient()
        {
            if (CurrentOperator.Operator == null)
                return;
            log.Debug("UnRegisterClient Client Started: " + CurrentOperator.Operator.OperatorName);
            var operatorNameId = CurrentOperator.Operator.OperatorName + "-" + CurrentOperator.Operator.OperatorId;
            try
            {
                if (client == null)
                {
                    CreateClient();
                    log.Debug("UnRegisterClient. Client was NULL. After Client Creation");
                }
                else
                if (client != null && client.InnerDuplexChannel.State == System.ServiceModel.CommunicationState.Faulted)
                {
                    client.Abort();
                    client = null;
                    CreateClient();
                    log.Debug("UnRegisterClient. Client was NOT NULL. After Client Creation");
                }
                client.UnRegisterClient(operatorNameId);
                log.Debug("UnRegisterClient. After UnRegister Client");
            }
            catch (Exception ex)
            {
                log.Error("UnRegisterClient Exception: " + ex.Message);
            }
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
