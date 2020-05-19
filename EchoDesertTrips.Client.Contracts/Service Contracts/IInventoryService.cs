using Core.Common.Contracts;
using Core.Common.Exceptions;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Contracts
{
    [ServiceContract]
    public interface IInventoryService : IServiceContract
    {
        [OperationContract]
        InventoryData GetInventoryData();

        [OperationContract]
        Hotel UpdateHotel(Hotel hotel);

        //[OperationContract]
        //Nationality UpdateNationality(Nationality nationality);

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
        Agency UpdateAgent(Agent agent);

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

        [OperationContract]
        Hotel GetHotelById(int id);

        [OperationContract]
        TourType GetTourTypeById(int id);

        [OperationContract]
        Agency GetAgencyById(int id);
    }
}
