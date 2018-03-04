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
    class OrderClient : UserClientBase<IOrderService>, IOrderService
    {
        public void CancelReservation(int reservationId)
        {
            Channel.CancelReservation(reservationId);
        }

        public Reservation[] GetAllReservations()
        {
            return Channel.GetAllReservations();
        }

        public Reservation[] GetDeadReservations()
        {
            return Channel.GetDeadReservations();
        }

        public ReservationData UpdateReservation( Reservation reservation)
        {
            return Channel.UpdateReservation(reservation);
        }

        //public Reservation[] GetReservationsForDay(DateTime Day)
        //{
        //    return Channel.GetReservationsForDay(Day);
        //}

        public Reservation[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo)
        {
            return Channel.GetReservationsForDayRange(DayFrom, DayTo);
        }

        public void DeleteTour(Tour Tour)
        {
            Channel.DeleteTour(Tour);
        }

        public void DeleteCustomer(Customer customer)
        {
            Channel.DeleteCustomer(customer);
        }

        public Reservation GetReservation(int ReservationId)
        {
            return Channel.GetReservation(ReservationId);
        }

        //public ReservationData EditReservation(int ReservationId)
        //{
        //    return Channel.EditReservation(ReservationId);
        //}

        public Task<Reservation[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo)
        {
            return Channel.GetReservationsForDayRangeAsynchronous(DayFrom, DayTo);
        }
    }
}
