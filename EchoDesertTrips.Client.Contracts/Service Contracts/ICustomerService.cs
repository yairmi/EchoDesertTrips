using System.Collections.Generic;
using Core.Common.Contracts;
using Core.Common.Exceptions;
using EchoDesertTrips.Client.Entities;
using System.ServiceModel;

namespace EchoDesertTrips.Client.Contracts
{
    [ServiceContract]
    public interface ICustomerService : IServiceContract
    {
        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Customer GetCustomer(int CustomerId);

        [OperationContract]
        Customer[] GetAllCustomers();

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Customer UpdateCustomer(Customer customer);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        List<Customer> UpdateCustomers(List<Customer> customers);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteCustomer(Customer customer);
    }
}
