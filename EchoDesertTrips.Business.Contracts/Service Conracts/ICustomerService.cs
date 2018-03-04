using System.Collections.Generic;
using Core.Common.Exceptions;
using EchoDesertTrips.Business.Entities;
using System.ServiceModel;

namespace EchoDesertTrips.Business.Contracts
{
    [ServiceContract]
    public interface ICustomerService
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
        void DeleteCustomer(Customer customer);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        List<Customer> UpdateCustomers(List<Customer> customers);
    }
}
