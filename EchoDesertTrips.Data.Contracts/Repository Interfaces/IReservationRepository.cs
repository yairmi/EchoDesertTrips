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
        Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo);
        Reservation[] GetReservationsByGroupId(int GroupId);
        Reservation[] GetCustomersByReservationGroupId(int GroupId);
        Reservation[] GetReservationsByIds(List<int> idList);
        void RemoveReservation(int reservationId);
        Reservation UpdateReservation(Reservation reservation);
        Reservation AddReservation(Reservation reservation);
    }
}
