using Core.Common.Exceptions;
using EchoDesertTrips.Business.Entities;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Contracts
{
    [ServiceContract]
    public interface IInventoryService
    {
        [OperationContract]
        InventoryData GetInventoryData();

        [OperationContract]
        Hotel UpdateHotel(Hotel hotel);

        [OperationContract]
        TourType UpdateTourType(TourType tourType);

        [OperationContract]
        Optional UpdateOptional(Optional optional);

        [OperationContract]
        Hotel[] GetAllHotels();

        [OperationContract]
        Agency[] GetAllAgencies();

        [OperationContract]
        Agency UpdateAgency(Agency agency);

        [OperationContract]
        Agent UpdateAgent(Agent agent);

        [OperationContract]
        TourType[] GetAllTourTypes();

        [OperationContract]
        Optional[] GetAllOptionals();

        [OperationContract]
        RoomType[] GetAllRoomTypes();

        [OperationContract]
        RoomType UpdateRoomType(RoomType roomType);

        [OperationContract]
        void DeleteOptional(Optional optional);

        [OperationContract]
        void DeleteRoomType(RoomType roomType);

        [OperationContract]
        void DeleteTourType(TourType tourType);

        [OperationContract]
        void DeleteAgency(Agency agency);

        [OperationContract]
        void DeleteAgent(Agent agent);

        [OperationContract]
        void DeleteHotel(Hotel hotel);

        [OperationContract]
        Task<InventoryData> GetInventoryDataAsynchronous();

        [OperationContract]
        Hotel GetHotelById(int id);

        [OperationContract]
        TourType GetTourTypeById(int id);

    }
}
