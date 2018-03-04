using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface ITourRepository : IDataRepository<Tour>
    {
        void RemoveTour(int tourId);
    }
}
