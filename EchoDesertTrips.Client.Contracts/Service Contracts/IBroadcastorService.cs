using System.ServiceModel;

namespace EchoDesertTrips.Client.Contracts
{
    //Since it is a duplex service we need to decorate it with CallbackContract
    [ServiceContract(CallbackContract = typeof(IBroadcastorCallBack))]
    public interface IBroadcastorService
    {
        [OperationContract]
        bool RegisterClient(string clientName);

        [OperationContract]
        bool UnRegisterClient(string clientName);

        [OperationContract(IsOneWay = true)]
        void NotifyServer(EventDataType eventData);
    }

    public interface IBroadcastorCallBack
    {
        [OperationContract(IsOneWay = true)]
        void BroadcastToClient(BroadcastMessage Message);
    }
}
