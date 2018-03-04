using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.Exceptions;
using EchoDesertTrips.Business.Common;
using EchoDesertTrips.Business.Contracts;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.DTOs;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple,
    ReleaseServiceInstanceOnTransactionComplete = false)]
    public class OperationManager : ManagerBase, IOperationService
    {
        [Import]
        private IDataRepositoryFactory _DataRepositoryFactory;

        [Import]
        public IBusinessEngineFactory _BusinessEngineFactory;

        public OperationManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public OperationManager(IDataRepositoryFactory dataRepositoryFactory)
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

                IEnumerable<Customer> customers = customerRepository.Get();
                return customers.ToArray();
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
                    NotFoundException ex = new NotFoundException(string.Format("Trip ID: {0} was not foudn", CustomerId));
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

        public Reservation[] GetDeadReservations()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository orderRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                IEnumerable<Reservation> reservations =
                    orderRepository.GetReservationByEndDate(DateTime.Now.AddDays(-1));

                return (reservations != null ? reservations.ToArray() : null);
            });
        }

        public void CancelReservation(int reservationId)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository orderRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();

                Reservation reservation = orderRepository.Get(reservationId);

                if (reservation == null)
                {
                    NotFoundException ex = new NotFoundException(string.Format("No order found for ID '{0}'", reservationId));

                    throw new FaultException<NotFoundException>(ex, ex.Message);
                }

                orderRepository.Remove(reservationId);
            });
        }

        public CustomerOrderData[] GetCurrentReservations()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository orderRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                List<CustomerOrderData> reservationData = new List<CustomerOrderData>();
                IEnumerable<CustomerOrderInfo> customersOrderInfo = orderRepository.GetCustomerOrderInfo();
                foreach (CustomerOrderInfo c in customersOrderInfo)
                {
                    reservationData.Add(new CustomerOrderData()
                    {
                        Customer = c.customer,
                        Trip = c.trip,
                        Agency = c.agency,
                        Agent = c.agent
                    });
                }

                return reservationData.ToArray();
            });
        }

        public CustomerOrderData UpdateReservation(CustomerOrderData customerOrderData)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IReservationRepository orderRepository = _DataRepositoryFactory.GetDataRepository<IReservationRepository>();
                ICustomerRepository customerRepository = _DataRepositoryFactory.GetDataRepository<ICustomerRepository>();
                CustomerOrderData customerOrderData_ = new CustomerOrderData();
                Customer updateEntity = null;

                if (customerOrderData.Customer.CustomerId == 0)
                {
                    updateEntity = customerRepository.Add(customerOrderData.Customer);
                }
                else
                {
                    updateEntity = customerRepository.Update(customerOrderData.Customer);
                }
                customerOrderData_.Reservation = orderRepository.Update(customerOrderData.Reservation);
                customerOrderData_.Customer = updateEntity;
                customerOrderData_.Trip = customerOrderData.Trip;

                return customerOrderData_;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        public void DeleteTrip(Tour trip)
        {
            ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tripRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                tripRepository.Remove(trip.TourId);
            });
        }

        //WCF is authorize this operation only for users that are in the car rental admin windows group
        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        //WCF authorize against any other user. this one is not againt a role. It is againt a user name
        //[PrincipalPermission(SecurityAction.Demand, Name = Security.EchoDesertTripsUser)]
        public Tour[] GetAllTrips()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tripRepository =
                     _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                IEnumerable<Tour> trips = tripRepository.Get();
                return trips.ToArray();
            });
        }

        public TourType[] GetAllTripTypes()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourTypeRepository tripTypeRepository =
                     _DataRepositoryFactory.GetDataRepository<ITourTypeRepository>();

                IEnumerable<TourType> tripTypes = tripTypeRepository.Get();
                return tripTypes.ToArray();
            });
        }

        public Tour[] GetOrderedTrips()
        {
            //Get a specific engine
            ITourEngine tripOrderedEngine = _BusinessEngineFactory.GetBusinessEngine<ITourEngine>();
            return null; //TODO : change to Trip[] return type
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        //[PrincipalPermission(SecurityAction.Demand, Name = Security.EchoDesertTripsUser)]
        public Tour GetTrip(int TripId)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tripRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                Tour trip = tripRepository.Get(TripId);

                if (trip == null)
                {
                    NotFoundException ex = new NotFoundException(string.Format("Trip ID: {0} was not foudn", TripId));
                    //Wrap this in something called SOAP vault, which will get transmitted through
                    //The SOAP message, and then the client will know how to handle that later.
                    //WCF allows as to do this with something called a FaultException of T.
                    throw new FaultException<NotFoundException>(ex, ex.Message);
                    //The client will be able to trap specifically and translate back to NotFoundException
                    //Also, throwing a FaultException of T will not fault the proxy, which means that the proxy will be usable
                }

                return trip;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        //[PrincipalPermission(SecurityAction.Demand, Role = Security.EchoDesertTripsAdminRole)]
        public Tour UpdateTrip(Tour trip)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                ITourRepository tripRepository =
                    _DataRepositoryFactory.GetDataRepository<ITourRepository>();

                Tour updateEntity = null;

                if (trip.TourId == 0)
                {
                    updateEntity = tripRepository.Add(trip);
                }
                else
                {
                    updateEntity = tripRepository.Update(trip);
                }

                return updateEntity;
            });
        }
    }
}
