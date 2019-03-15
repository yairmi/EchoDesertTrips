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

        /*public void RegisterClient(string operatorNameId)
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

            //var operatorNameId = CurrentOperator.Operator.OperatorName + "-" + CurrentOperator.Operator.OperatorId;

            client.RegisterClient(operatorNameId);
        }

        public void UnRegisterClient(Operator Operator)
        {
            if (Operator == null)
                return;
            log.Debug("UnRegisterClient Client Started: " + Operator.OperatorName);
            var operatorNameId = Operator.OperatorName + "-" + Operator.OperatorId;
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
            var cb = new BroadcastorCallback();
            cb.SetHandler(HandleBroadcast);

            var context =
                new System.ServiceModel.InstanceContext(new BroadcastorCallback());
            client = new BroadcastorServiceClient(context);
        }

        public void HandleBroadcast(object sender, BroadcastMessage Message)
        {
            log.Debug("HandleBroadcast");
            if (Message.MessageType == eMsgTypes.E_RESERVATION)
            {
                foreach (var reservation in Message.ReservationsResult)
                    _eventAggregator.GetEvent<ReservationUpdatedEvent>().Publish(reservation);
                foreach (var reservationId in Message.ReservationsIdsToDelete)
                    _eventAggregator.GetEvent<ReservationRemovedEvent>().Publish(reservationId);
            }
            else if (Message.MessageType == eMsgTypes.E_INVENTORY)
                UpdateInventory(Message.Inventories);
        }*/
    }
}
