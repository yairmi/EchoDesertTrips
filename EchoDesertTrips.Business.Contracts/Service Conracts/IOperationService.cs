using Core.Common.Exceptions;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Contracts
{
    [ServiceContract]
    public interface IOperationService
    {
        //------------------ Customer ------------------
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

        //------------------ Order ------------------

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetDeadReservations();

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void CancelReservation(int reservationId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        CustomerOrderData[] GetCurrentReservations();

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        CustomerOrderData UpdateReservation(CustomerOrderData Reservation);

        //------------------ Trip ------------------

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Tour GetTrip(int TripId);

        [OperationContract]
        Tour[] GetAllTrips();

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Tour UpdateTrip(Tour trip);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteTrip(Tour trip);

        [OperationContract]
        Tour[] GetOrderedTrips();

        [OperationContract]
        TourType[] GetAllTripTypes();
    }
}
