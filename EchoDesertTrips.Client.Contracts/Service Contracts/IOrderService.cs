using Core.Common.Contracts;
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
        Reservation DeleteReservation(int reservationId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Reservation[] GetAllReservations();

        [OperationContract]
        [FaultContract(typeof(UpdateConcurrencyException))]
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

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Task<Customer[]> GetCustomersByReservationGroupIdAsynchronous(int GroupId);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        Task<Reservation[]> GetReservationsByIdsAsynchronous(List<int> idList);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Reservation EditReservation(int ReservationID, Operator o);

        [OperationContract]
        [FaultContract(typeof(NotFoundException))]
        void UnLock(int ReservationID);
    }
}
