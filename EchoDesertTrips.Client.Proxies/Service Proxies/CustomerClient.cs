using System.Collections.Generic;
using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    [Export(typeof(ICustomerService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerClient : UserClientBase<ICustomerService>, ICustomerService
    {
        public void DeleteCustomer(Customer customer)
        {
            Channel.DeleteCustomer(customer);
        }

        public Customer[] GetAllCustomers()
        {
            return Channel.GetAllCustomers();
        }

        public Customer GetCustomer(int CustomerId)
        {
            return Channel.GetCustomer(CustomerId);
        }

        public Customer UpdateCustomer(Customer customer)
        {
            return Channel.UpdateCustomer(customer);
        }

        public List<Customer> UpdateCustomers(List<Customer> customers)
        {
            return Channel.UpdateCustomers(customers);
        }

    }
}
