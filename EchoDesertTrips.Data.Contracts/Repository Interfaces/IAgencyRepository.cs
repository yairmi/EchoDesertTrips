using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface IAgencyRepository : IDataRepository<Agency>
    {
        Agency UpdateAgency(Agency agency);
    }
}
