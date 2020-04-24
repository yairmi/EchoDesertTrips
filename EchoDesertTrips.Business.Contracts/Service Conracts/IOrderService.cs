using Core.Common.Exceptions;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
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
        Reservation DeleteReservation(int reservationId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetAllReservations();

        [OperationContract]
        [FaultContract(typeof(UpdateConcurrencyException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Reservation UpdateReservation(Reservation reservation);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        ReservationDTO[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Task<ReservationDTO[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetReservationsByGroupId(int GroupId);

        //[OperationContract]
        //[FaultContract(typeof(NotFoundException))]
        //Reservation[] GetCustomersByReservationGroupId(int GroupId);

        //[OperationContract]
        //[FaultContract(typeof(NotFoundException))]
        //Task<Reservation[]> GetReservationsCustomersByReservationGroupIdAsynchronous(int GroupId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Task<Customer[]> GetCustomersByReservationGroupIdAsynchronous(int GroupId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Task<ReservationDTO[]> GetReservationsByIdsAsynchronous(List<int> idList);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Reservation EditReservation(int ReservationID, Operator o);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        void UnLock(int ReservationID);
    }
}
