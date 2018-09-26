using Core.Common.Contracts;
using EchoDesertTrips.Business.Contracts;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.DTOs;
using System;
using System.Collections.Generic;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface IReservationRepository : IDataRepository<Reservation>
    {
        Reservation[] GetReservationByEndDate(DateTime date);
        IEnumerable<CustomerOrderInfo> GetCustomerOrderInfo();
        Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo);
        void RemoveReservation(int reservationId);
        ReservationData UpdateReservation(Reservation reservation);
        Reservation AddReservation(Reservation reservation);
        Reservation[] GetReservationsByGroupId(int GroupId);
    }
}
