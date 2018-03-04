using Core.Common.Contracts;
using Core.Common.Exceptions;
using EchoDesertTrips.Client.Entities;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Contracts
{
    [ServiceContract]
    public interface ITourService : IServiceContract
    {
        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Tour GetTour(int TourId);

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

        [OperationContract]
        Task DeleteTourAsync(Tour tour);

        [OperationContract]
        Task<Tour> GetTourAsync(int TourId);

        [OperationContract]
        Task<Tour[]> GetAllToursAsync();

        [OperationContract]
        Task<Tour> UpdateTourAsync(Tour tour);
    }
}
