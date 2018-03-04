using Core.Common.Exceptions;
using EchoDesertTrips.Business.Entities;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Contracts
{
    [ServiceContract]
    public interface ITourService
    {
        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Tour GetTour(int tourId);

        [OperationContract]
        Tour[] GetAllTours();

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Tour UpdateTour(Tour tour);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteTour(Tour tour);

        [OperationContract]
        Tour[] GetOrderedTours();

        [OperationContract]
        TourType[] GetAllTourTypes();
    }
}
