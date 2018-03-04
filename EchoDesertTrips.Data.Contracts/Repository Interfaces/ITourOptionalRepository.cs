using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using System.Collections.Generic;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface ITourOptionalRepository : IDataRepository<TourOptional>
    {
        IEnumerable<TourOptional> GetTourOptionalsByTourId(int tourId);
    }
}
