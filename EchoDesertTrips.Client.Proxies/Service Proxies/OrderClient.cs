using EchoDesertTrips.Client.Contracts;
using System.ComponentModel.Composition;
using EchoDesertTrips.Client.Entities;
using Core.Common.ServiceModel;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        public ReservationDTO[] GetReservationsForDayRange(DateTime DayFrom, DateTime DayTo)
        {
            return Channel.GetReservationsForDayRange(DayFrom, DayTo);
        }

        public Task<ReservationDTO[]> GetReservationsForDayRangeAsynchronous(DateTime DayFrom, DateTime DayTo)
        {
            return Channel.GetReservationsForDayRangeAsynchronous(DayFrom, DayTo);
        }

        public Reservation[] GetReservationsByGroupId(int GroupId)
        {
            return Channel.GetReservationsByGroupId(GroupId);
        }

        //public Reservation[] GetCustomersByReservationGroupId(int GroupId)
        //{
        //    return Channel.GetCustomersByReservationGroupId(GroupId);
        //}

        //public Task<Reservation[]> GetReservationsCustomersByReservationGroupIdAsynchronous(int GroupID)
        //{
        //    return Channel.GetReservationsCustomersByReservationGroupIdAsynchronous(GroupID);
        //}

        public Task<Customer[]> GetCustomersByReservationGroupIdAsynchronous(int GroupId)
        {
            return Channel.GetCustomersByReservationGroupIdAsynchronous(GroupId);
        }

        public Task<Reservation[]> GetReservationsByIdsAsynchronous(List<int> idList)
        {
            return Channel.GetReservationsByIdsAsynchronous(idList);
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
