﻿using Core.Common.Contracts;
using Core.Common.Exceptions;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Contracts
{
    [ServiceContract]
    public interface IOrderService : IServiceContract
    {
        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetDeadReservations();

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void CancelReservation(int reservationId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetAllReservations();

        [OperationContract]
        [FaultContract(typeof(UpdateConcurrencyException))]
        ReservationData UpdateReservation(Reservation reservation);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteTour(Tour Tour);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteCustomer(Customer customer);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Reservation GetReservation(int ReservationId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Task<Reservation[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetReservationsByGroupId(int GroupId);
    }
}
