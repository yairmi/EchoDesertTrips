using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;

namespace EchoDesertTrips.Data.Contracts.Repository_Interfaces
{
    public interface IReservationRepository : IDataRepository<Reservation>
    {
        Reservation[] GetReservationByEndDate(DateTime date);
        Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo, int customersAmount);
        Reservation[] GetReservationsByGroupId(int GroupId);
        Reservation[] GetReservationsCustomersByReservationGroupId(int GroupId);
        Reservation[] GetReservationsByIds(List<int> idList, int customersAmount);
        Customer[] GetCustomersByReservationGroupId(int GroupID);
        void RemoveReservation(int reservationId);
        Reservation UpdateReservation(Reservation reservation);
        Reservation AddReservation(Reservation reservation);
    }
}
