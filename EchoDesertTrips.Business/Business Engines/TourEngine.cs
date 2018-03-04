using EchoDesertTrips.Business.Common;
using EchoDesertTrips.Business.Entities;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Business.Business_Engines
{
    [Export(typeof(ITourEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourEngine : ITourEngine
    {
        public Tour UpdateTour(Tour tour)
        {
            return null;
        }
    }
}
