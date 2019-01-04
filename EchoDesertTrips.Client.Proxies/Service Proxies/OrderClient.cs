using EchoDesertTrips.Client.Contracts;
using System.ComponentModel.Composition;
using EchoDesertTrips.Client.Entities;
using Core.Common.ServiceModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    [Export(typeof(IOrderService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OrderClient : UserClientBase<IOrderService>, IOrderService
    {
        public Reservation DeleteReservation(int reservationId)
        {
            return Channel.DeleteReservation(reservationId);
        }

        public Reservation[] GetAllReservations()
        {
            return Channel.GetAllReservations();
        }

        public Reservation[] GetDeadReservations()
        {
            return Channel.GetDeadReservations();
        }

        public Reservation UpdateReservation( Reservation reservation)
        {
            return Channel.UpdateReservation(reservation);
        }

        public Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo)
        {
            log.Debug("OrderClient: GetReservationsForDayRange Start");
            var reservations = Channel.GetReservationsForDayRange(DayFrom, DayTo);
            log.Debug("OrderClient: GetReservationsForDayRange End");
            return reservations;
        }

        public void DeleteTour(Tour Tour)
        {
            Channel.DeleteTour(Tour);
        }

        public void DeleteCustomer(Customer customer)
        {
            Channel.DeleteCustomer(customer);
        }

        //public Reservation GetReservation(int ReservationId)
        //{
        //    return Channel.GetReservation(ReservationId);
        //}

        public Task<Reservation[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo)
        {
            return Channel.GetReservationsForDayRangeAsynchronous(DayFrom, DayTo);
        }

        public Reservation[] GetReservationsByGroupId(int GroupId)
        {
            return Channel.GetReservationsByGroupId(GroupId);
        }

        public Reservation[] GetCustomersByReservationGroupId(int GroupId)
        {
            return Channel.GetCustomersByReservationGroupId(GroupId);
        }

        public Task<Reservation[]> GetCustomersByReservationGroupIdAsynchronous(int GroupID)
        {
            return Channel.GetCustomersByReservationGroupIdAsynchronous(GroupID);
        }

        public Reservation EditReservation(int ReservationID, Operator o)
        {
            return Channel.EditReservation(ReservationID, o);
        }

        public void UnLock(int ReservationID)
        {
            Channel.UnLock(ReservationID);
        }
    }
}
