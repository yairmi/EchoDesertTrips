using EchoDesertTrips.Client.Contracts;
using System.Threading.Tasks;
using EchoDesertTrips.Client.Entities;
using System.ComponentModel.Composition;
using Core.Common.ServiceModel;
using System;
using System.Collections.Generic;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    [Export(typeof(IInventoryService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class InventoryClient : UserClientBase<IInventoryService>, IInventoryService
    {
        public Hotel[] GetAllHotels()
        {
            return Channel.GetAllHotels();
        }

        public Agency[] GetAllAgencies()
        {
            return Channel.GetAllAgencies();
        }

        public Hotel UpdateHotel(Hotel hotel)
        {
            return Channel.UpdateHotel(hotel);
        }

        public TourType UpdateTourType(TourType tourType)
        {
            return Channel.UpdateTourType(tourType);
        }

        public Optional UpdateOptional(Optional optional)
        {
            return Channel.UpdateOptional(optional);
        }

        public InventoryData GetInventoryData()
        {
            return Channel.GetInventoryData();
        }

        public Agency UpdateAgency(Agency agency)
        {
            return Channel.UpdateAgency(agency);
        }

        public Agency UpdateAgent(Agent agent)
        {
            return Channel.UpdateAgent(agent);
        }

        public TourType[] GetAllTourTypes()
        {
            return Channel.GetAllTourTypes();
        }

        public Optional[] GetAllOptionals()
        {
            return Channel.GetAllOptionals();
        }

        public RoomType[] GetAllRoomTypes()
        {
            return Channel.GetAllRoomTypes();
        }

        public RoomType UpdateRoomType(RoomType roomType)
        {
            return Channel.UpdateRoomType(roomType);
        }

        public void DeleteOptional(Optional optional)
        {
            Channel.DeleteOptional(optional);
        }

        public void DeleteRoomType(RoomType roomType)
        {
            Channel.DeleteRoomType(roomType);
        }

        public void DeleteTourType(TourType tourType)
        {
            Channel.DeleteTourType(tourType);
        }

        public void DeleteAgency(Agency agency)
        {
            Channel.DeleteAgency(agency);
        }

        public void DeleteAgent(Agent agent)
        {
            Channel.DeleteAgent(agent);
        }

        public void DeleteHotel(Hotel hotel)
        {
            Channel.DeleteHotel(hotel);
        }

        public Task<InventoryData> GetInventoryDataAsynchronous()
        {
            return Channel.GetInventoryDataAsynchronous();
        }

        public Hotel GetHotelById(int id)
        {
            return Channel.GetHotelById(id);
        }

        public TourType GetTourTypeById(int id)
        {
            return Channel.GetTourTypeById(id);
        }
    }
}
