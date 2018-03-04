using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    [Export(typeof(ITourService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class TourClient : UserClientBase<ITourService>, ITourService
    {
        public void DeleteTour(Tour Tour)
        {
            Channel.DeleteTour(Tour);
        }

        public Task DeleteTourAsync(Tour Tour)
        {
            return Channel.DeleteTourAsync(Tour);
        }

        public Tour[] GetAllTours()
        {
            return Channel.GetAllTours();
        }

        public Task<Tour[]> GetAllToursAsync()
        {
            return Channel.GetAllToursAsync();
        }

        public TourType[] GetAllTourTypes()
        {
            return Channel.GetAllTourTypes();
        }

        public Tour[] GetOrderedTours()
        {
            return Channel.GetOrderedTours();
        }

        public Tour GetTour(int TourId)
        {
            return Channel.GetTour(TourId);
        }

        public Task<Tour> GetTourAsync(int TourId)
        {
            return Channel.GetTourAsync(TourId);
        }

        public Tour UpdateTour(Tour Tour)
        {
            return Channel.UpdateTour(Tour);
        }

        public Task<Tour> UpdateTourAsync(Tour Tour)
        {
            return Channel.UpdateTourAsync(Tour);
        }
    }
}
