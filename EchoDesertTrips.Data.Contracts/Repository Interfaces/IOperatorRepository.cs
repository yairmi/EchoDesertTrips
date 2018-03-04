using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface IOperatorRepository : IDataRepository<Operator>
    {
        Operator GetOperator(string OperatorName, string OperatorPassword);
    }
}
