using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    [Export(typeof(IOperationService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class OperationClient : UserClientBase<IOperationService>, IOperationService
    {
        //----------------- Customer -----------------
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

        //----------------- Order -----------------

        public void CancelReservation(int reservationId)
        {
            Channel.CancelReservation(reservationId);
        }

        public CustomerOrderData[] GetCurrentReservations()
        {
            return Channel.GetCurrentReservations();
        }

        public Reservation[] GetDeadReservations()
        {
            return Channel.GetDeadReservations();
        }

        public CustomerOrderData UpdateReservation(CustomerOrderData reservation)
        {
            return Channel.UpdateReservation(reservation);
        }

        //----------------- Trip -----------------

        public void DeleteTrip(Tour trip)
        {
            Channel.DeleteTrip(trip);
        }

        public Tour[] GetAllTrips()
        {
            return Channel.GetAllTrips();
        }

        public TourType[] GetAllTripTypes()
        {
            return Channel.GetAllTripTypes();
        }

        public Tour[] GetOrderedTrips()
        {
            return Channel.GetOrderedTrips();
        }

        public Tour GetTrip(int TripId)
        {
            return Channel.GetTrip(TripId);
        }

        public Tour UpdateTrip(Tour trip)
        {
            return Channel.UpdateTrip(trip);
        }
    }
}
