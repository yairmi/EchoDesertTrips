using Core.Common.Exceptions;
using EchoDesertTrips.Business.Contracts.Data_Contracts;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Contracts
{
    [ServiceContract]
    public interface IOrderService
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
        //[FaultContract(typeof(NotFoundException))]
        [FaultContract(typeof(UpdateConcurrencyException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
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
        Reservation GetReservation(int ReservationId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Task<Reservation[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetReservationsByGroupId(int GroupId);
    }
}
