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
        InventoryData GetHotelsData();

        [OperationContract]
        InventoryData GetInventoryData();

        [OperationContract]
        void UpdateHotelAndRoomTypes(List<HotelRoomType> hotelRoomTypes);

        [OperationContract]
        Hotel UpdateHotel(Hotel hotel);

        //[OperationContract]
        //Nationality UpdateNationality(Nationality nationality);

        [OperationContract]
        TourType UpdateTourType(TourType tourType);

        [OperationContract]
        Optional UpdateOptional(Optional optional);

        [OperationContract]
        Agency[] GetAllAgencies();

        [OperationContract]
        Agency UpdateAgency(Agency agency);

        [OperationContract]
        Agent UpdateAgent(Agent agent);

        //[OperationContract]
        //Nationality[] GetAllNationalities();

        [OperationContract]
        TourType[] GetAllTourTypes();

        [OperationContract]
        Optional[] GetAllOptionals();

        //[OperationContract]
        //TourDestination[] GetAllTourDestinations();

        //[OperationContract]
        //TourDestination UpdateTourDestination(TourDestination tourDestination);

        [OperationContract]
        RoomType[] GetAllRoomTypes();

        [OperationContract]
        RoomType UpdateRoomType(RoomType roomType);

        [OperationContract]
        void DeleteOptional(Optional optional);

        //[OperationContract]
        //void DeleteTourDestination(TourDestination tourDestination);

        [OperationContract]
        void DeleteRoomType(RoomType roomType);

        [OperationContract]
        void DeleteTourType(TourType tourType);

        //[OperationContract]
        //void DeleteNationality(Nationality nationality);

        [OperationContract]
        void DeleteAgency(Agency agency);

        [OperationContract]
        void DeleteAgent(Agent agent);

        [OperationContract]
        void DeleteHotel(Hotel hotel);

        [OperationContract]
        Task<InventoryData> GetInventoryDataAsynchronous();

    }
}
