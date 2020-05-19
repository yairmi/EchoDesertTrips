using EchoDesertTrips.Business.Contracts;
using System.Collections.Generic;
using System.Linq;
using EchoDesertTrips.Business.Entities;
using Core.Common.Core;
using System.ComponentModel.Composition;
using Core.Common.Contracts;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using Core.Common.Exceptions;
using System.ServiceModel;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple,
    ReleaseServiceInstanceOnTransactionComplete = false)]
    public class CustomerManager : ManagerBase, ICustomerService
    {
        public CustomerManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public CustomerManager(IDataRepositoryFactory dataRepositoryFactory)
        {
            _DataRepositoryFactory = dataRepositoryFactory;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void DeleteCustomer(Customer customer)
        {
            ExecuteFaultHandledOperation(() =>
            {
                ICustomerRepository customerRepository =
                    _DataRepositoryFactory.GetDataRepository<ICustomerRepository>();

                customerRepository.Remove(customer.CustomerId);
            });
        }

        public Customer[] GetAllCustomers()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ICustomerRepository customerRepository =
                     _DataRepositoryFactory.GetDataRepository<ICustomerRepository>();
                var customersData = new List<Customer>();
                IEnumerable<Customer> customers = customerRepository.Get();

                foreach (var c in customers)
                {
                    customersData.Add(c);
                }

                return customersData.ToArray();
            });
        }

        public Customer GetCustomer(int CustomerId)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ICustomerRepository customerRepository =
                    _DataRepositoryFactory.GetDataRepository<ICustomerRepository>();

                Customer customer = customerRepository.Get(CustomerId);

                if (customer == null)
                {
                    NotFoundException ex = new NotFoundException($"CustomerId: {CustomerId} was not foudn");
                    //Wrap this in something called SOAP vault, which will get transmitted through
                    //The SOAP message, and then the client will know how to handle that later.
                    //WCF allows as to do this with something called a FaultException of T.
                    throw new FaultException<NotFoundException>(ex, ex.Message);
                    //The client will be able to trap specifically and translate back to NotFoundException
                    //Also, throwing a FaultException of T will not fault the proxy, which means that the proxy will be usable
                }

                return customer;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public Customer UpdateCustomer(Customer customer)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ICustomerRepository customerRepository =
                    _DataRepositoryFactory.GetDataRepository<ICustomerRepository>();

                Customer updateEntity = null;

                if (customer.CustomerId == 0)
                {
                    updateEntity = customerRepository.Add(customer);
                }
                else
                {
                    updateEntity = customerRepository.Update(customer);
                }

                return updateEntity;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public List<Customer> UpdateCustomers(List<Customer> customers)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ICustomerRepository customerRepository =
                    _DataRepositoryFactory.GetDataRepository<ICustomerRepository>();

                List<Customer> retCustomers = new List<Customer>(); 

                foreach (var customer in customers)
                {

                    Customer updateEntity = null;

                    if (customer.CustomerId == 0)
                    {
                        updateEntity = customerRepository.Add(customer);
                    }
                    else
                    {
                        updateEntity = customerRepository.Update(customer);
                    }

                    retCustomers.Add(updateEntity);
                }

                return retCustomers;
            });
        }

        [Import]
        private IDataRepositoryFactory _DataRepositoryFactory;
    }
}
