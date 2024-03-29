﻿using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Z.EntityFramework.Plus;
using System.Linq.Expressions;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IReservationRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationRepository : DataRepositoryBase<Reservation>, IReservationRepository
    {
        protected override DbSet<Reservation> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.ReservationSet;
        }

        protected override Expression<Func<Reservation, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.ReservationId == id);
        }

        protected override Reservation AddEntity(EchoDesertTripsContext entityContext, Reservation entity)
        {
            return null;
        }

        protected override Reservation UpdateEntity(EchoDesertTripsContext entityContext, Reservation reservation)
        {
            return GetEntity(entityContext, reservation.ReservationId);
        }

        protected override IEnumerable<Reservation> GetEntities(EchoDesertTripsContext entityContext)
        {
            return null;
        }

        protected override IEnumerable<Reservation> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;

        }

        public ReservationDTO[] GetReservationsByIds(List<int> idList)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                return (from e in entityContext.ReservationSet.Where(t => idList.Contains(t.ReservationId)) select e)
                    .IncludeOptimized(a => a.Agency)
                    .IncludeOptimized(o => o.Customers)
                    .IncludeOptimized(o => o.Group)
                    .IncludeOptimized(o => o.Tours.Select(t => t.TourType))
                    .IncludeOptimized(o => o.Tours.Select(th => th.TourHotels.Select(h => h.Hotel)))
                    .Select(reservation => new ReservationDTO()
                    {
                        ReservationId = reservation.ReservationId,
                        FirstName = reservation.Customers.Count() == 0 ? string.Empty : reservation.Customers.FirstOrDefault().FirstName,
                        LastName = reservation.Customers.Count() == 0 ? string.Empty : reservation.Customers.FirstOrDefault().LastName,
                        Phone1 = reservation.Customers.Count() == 0 ? string.Empty : reservation.Customers.FirstOrDefault().Phone1,
                        HotelName = reservation.Tours.Count() == 0 ? string.Empty : reservation.Tours.FirstOrDefault().TourHotels.FirstOrDefault() == null ? "" : reservation.Tours.FirstOrDefault().TourHotels.FirstOrDefault().Hotel.HotelName,
                        AgencyName = reservation.Agency.AgencyName,
                        AdvancePayment = reservation.AdvancePayment,
                        TotalPrice = reservation.TotalPrice,
                        PickUpTime = reservation.PickUpTime,
                        Comments = reservation.Comments,
                        Messages = reservation.Messages,
                        Group = reservation.Group,
                        GroupID = reservation.GroupID,
                        ActualNumberOfCustomers = reservation.ActualNumberOfCustomers,
                        Car = reservation.Car,
                        Guide = reservation.Guide,
                        EndIn = reservation.EndIn,
                        Tours = reservation.Tours
                    }).ToArray();
            }
        }

        public Reservation[] GetReservationsByGroupId(int groupId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var query = (from e in entityContext.ReservationSet where e.GroupID == groupId select e);
                return IncludeNavigationProperties(query).ToArray();
            }
        }

        public ReservationDTO[] GetReservationsForDayRange(DateTime dayFrom, DateTime dayTo)
        {
            using (var entityContext = new EchoDesertTripsContext())
            {
                return ((from e in entityContext.ReservationSet where e.Tours.Any(t => t.StartDate >= dayFrom && t.StartDate <= dayTo) select e)
                    .IncludeOptimized(a => a.Agency)
                    .IncludeOptimized(o => o.Customers)
                    .IncludeOptimized(o => o.Group)
                    .IncludeOptimized(o => o.Tours.Select(t => t.TourType))
                    .IncludeOptimized(o => o.Tours.Select(th => th.TourHotels.Select(h => h.Hotel))))
                    .Select(reservation => new ReservationDTO()
                    {
                        ReservationId = reservation.ReservationId,
                        FirstName = reservation.Customers.Count() == 0 ? string.Empty : reservation.Customers.FirstOrDefault().FirstName,
                        LastName = reservation.Customers.Count() == 0 ? string.Empty : reservation.Customers.FirstOrDefault().LastName,
                        Phone1 = reservation.Customers.Count() == 0 ? string.Empty : reservation.Customers.FirstOrDefault().Phone1,
                        HotelName = reservation.Tours.Count() == 0 ? string.Empty : reservation.Tours.FirstOrDefault().TourHotels.FirstOrDefault() == null ? "" : reservation.Tours.FirstOrDefault().TourHotels.FirstOrDefault().Hotel.HotelName,
                        AgencyName = reservation.Agency.AgencyName,
                        AdvancePayment = reservation.AdvancePayment,
                        TotalPrice = reservation.TotalPrice,
                        PickUpTime = reservation.PickUpTime,
                        Comments = reservation.Comments,
                        Messages = reservation.Messages,
                        Group = reservation.Group,
                        GroupID = reservation.GroupID,
                        ActualNumberOfCustomers = reservation.ActualNumberOfCustomers,
                        FirstTourTypeName = reservation.Tours.Count() == 0 ? string.Empty : reservation.Tours.FirstOrDefault().TourType.TourTypeName,
                        Private = reservation.Tours.Count() == 0 ? false : reservation.Tours.FirstOrDefault().TourType.Private,
                        Car = reservation.Car,
                        Guide = reservation.Guide,
                        EndIn = reservation.EndIn,
                        Tours = reservation.Tours
                    }).ToArray();
            }
        }

        public Reservation[] GetReservationsCustomersByReservationGroupId(int GroupId)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                return (from e in entityContext.ReservationSet where e.GroupID == GroupId select e)
                            .Include(o => o.Customers).ToArray();
            }
        }

        public Customer[] GetCustomersByReservationGroupId(int GroupID)
        {
            using (var entityContext = new EchoDesertTripsContext())
            {
                var query = (from e in entityContext.ReservationSet where e.GroupID == GroupID select e)
                    .SelectMany(c => c.Customers);
                return query.ToArray();
            }
        }

        public Reservation[] GetReservationByEndDate(DateTime date)
        {
            return null;
        }

        public void RemoveReservation(int reservationId)
        {
            using (var entityContext = new EchoDesertTripsContext())
            {
                var existingReservation = GetEntity(entityContext, reservationId);
                entityContext.CustomerSet.RemoveRange(existingReservation.Customers);
                existingReservation.Tours.ForEach((tour) =>
                {
                    //For TourOptionals we have CASCADE ON DELETE
                    //entityContext.TourOptionalSet.RemoveRange(tour.TourOptionals);
                    tour.TourHotels.ForEach((tourHotel) =>
                    {
                        entityContext.TourHotelRoomTypesSet.RemoveRange(tourHotel.TourHotelRoomTypes);
                    });
                    entityContext.TourHotelSet.RemoveRange(tour.TourHotels);
                    entityContext.SubTourSet.RemoveRange(tour.SubTours);
                });
                entityContext.TourSet.RemoveRange(existingReservation.Tours);
                entityContext.Entry(existingReservation).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public Reservation UpdateReservation(Reservation reservation)
        {
            using (var entityContext = new EchoDesertTripsContext())
            {
                Reservation exsitingReservation = null;
                bool bRowVersionConflict = false;
                try
                {
                    ResetNavProperties(reservation);
                    exsitingReservation = (from e in entityContext.ReservationSet where e.ReservationId == reservation.ReservationId select e)
                                .Include(r => r.Customers)
                                .Include(t => t.Tours.Select(h => h.TourHotels.Select(th => th.TourHotelRoomTypes)))
                                .Include(t => t.Tours.Select(o => o.TourOptionals))
                                .Include(t => t.Tours.Select(s => s.SubTours))
                                .Include(o => o.Operator)
                                .FirstOrDefault();
                    if (exsitingReservation != null)
                    {
                        entityContext.ReservationSet.Attach(exsitingReservation);
                        entityContext.Entry(exsitingReservation).CurrentValues.SetValues(reservation);
                        //Handle Group
                        if (reservation.Group != null)
                        {
                            var existingGroup =
                                (from e in entityContext.GroupSet
                                 where e.ExternalId == reservation.Group.ExternalId
                                 select e).FirstOrDefault();
                            if (existingGroup == null)
                            {
                                exsitingReservation.Group = null;
                                exsitingReservation.GroupID = reservation.GroupID;
                            }
                            else
                            {
                                existingGroup.Updated = true;
                            }
                        }

                        //Handle Tours
                        foreach (var tour in reservation.Tours)
                        {
                            Tour exsitingTour = null;
                            if (tour.TourId == 0)//new tour
                            {
                                exsitingReservation.Tours.Add(tour);
                                continue;
                            }
                            else
                            {
                                exsitingTour = (from e in exsitingReservation.Tours where e.TourId == tour.TourId select e).FirstOrDefault();
                                if (exsitingTour != null)
                                {
                                    entityContext.Entry(exsitingTour).CurrentValues.SetValues(tour);
                                }
                                else
                                {
                                    log.Error($"ERROR! Fail to locate tour.TourId: {tour.TourId}");
                                }
                            }

                            //Sub Tours
                            if (tour.SubTours != null)
                            {
                                foreach (var subTour in tour.SubTours)
                                {
                                    if (subTour.SubTourId != 0)
                                    {
                                        var existingSubTour = (from e in exsitingTour.SubTours where e.SubTourId == subTour.SubTourId select e).FirstOrDefault();
                                        entityContext.Entry(existingSubTour).CurrentValues.SetValues(subTour);
                                    }
                                }
                            }

                            //Tour Optionals
                            //Add
                            if (tour.TourOptionals != null)
                            {
                                foreach (var tourOptional in tour.TourOptionals)
                                {
                                    var existingTourOptional = (from e in exsitingTour.TourOptionals where e.TourId == tourOptional.TourId && e.OptionalId == tourOptional.OptionalId select e).FirstOrDefault();
                                    if (existingTourOptional == null) //new TourOptional
                                    {
                                        entityContext.TourOptionalSet.Add(tourOptional);
                                    }
                                    else
                                        entityContext.Entry(existingTourOptional).CurrentValues.SetValues(tourOptional);
                                }
                            }

                            //Delete
                            if (exsitingTour.TourOptionals != null)
                            {
                                for (var i = exsitingTour.TourOptionals.Count - 1; i >= 0; i--)
                                {
                                    var exist = tour.TourOptionals.Exists(t => t.TourId == exsitingTour.TourOptionals[i].TourId
                                        && t.OptionalId == exsitingTour.TourOptionals[i].OptionalId);
                                    if (!exist)
                                    {
                                        entityContext.Entry(exsitingTour.TourOptionals[i]).State = EntityState.Deleted;
                                    }
                                }
                            }

                            //Tour Hotels
                            if (tour.TourHotels != null)
                            {
                                tour.TourHotels.ForEach((tourHotel) =>
                                {
                                    if (tourHotel.TourHotelId == 0)
                                    {
                                        exsitingTour.TourHotels.Add(tourHotel);
                                    }
                                    else
                                    {
                                        //Tour Hotel Room Types
                                        var existingTourHotel = (from e in exsitingTour.TourHotels where e.TourHotelId == tourHotel.TourHotelId select e).FirstOrDefault();
                                        if (existingTourHotel != null)
                                        {
                                            entityContext.Entry(existingTourHotel).CurrentValues.SetValues(tourHotel);
                                            if (tourHotel.TourHotelRoomTypes != null)
                                            {
                                                tourHotel.TourHotelRoomTypes.ForEach((tourHotelRoomType) =>
                                                {
                                                    if (tourHotelRoomType.TourHotelRoomTypeId == 0)
                                                    {
                                                        existingTourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                                                    }
                                                    else
                                                    {
                                                        var existingTourHotelRoomType = (from e in existingTourHotel.TourHotelRoomTypes where e.TourHotelRoomTypeId == tourHotelRoomType.TourHotelRoomTypeId select e).FirstOrDefault();
                                                        if (existingTourHotelRoomType != null)
                                                        {
                                                            entityContext.Entry(existingTourHotelRoomType).CurrentValues.SetValues(tourHotelRoomType);
                                                        }
                                                        else
                                                        {
                                                            log.Error($"ERROR! Fail to locate tourHotelRoomType.TourHotelRoomTypeId: {tourHotelRoomType.TourHotelRoomTypeId}");
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                        else
                                        {
                                            log.Error($"Fail to locate tourHotel.TourHotelId: {tourHotel.TourHotelId}");
                                        }
                                    }
                                });
                            }

                            if (exsitingTour != null)
                            {
                                for (var j = exsitingTour.TourHotels.Count() - 1; j >= 0; j--)
                                {
                                    var tourHotel = tour.TourHotels.FirstOrDefault(t =>
                                        t.TourHotelId == exsitingTour.TourHotels[j].TourHotelId);
                                    if (tourHotel != null)
                                    {
                                        for (var i = exsitingTour.TourHotels[j].TourHotelRoomTypes.Count - 1;
                                            i >= 0;
                                            i--)
                                        {
                                            var exist = tourHotel.TourHotelRoomTypes.FirstOrDefault(t =>
                                                t.TourHotelRoomTypeId == exsitingTour.TourHotels[j]
                                                    .TourHotelRoomTypes[i]
                                                    .TourHotelRoomTypeId);
                                            if (exist == null)
                                            {
                                                entityContext.Entry(exsitingTour.TourHotels[j].TourHotelRoomTypes[i])
                                                        .State
                                                    = EntityState.Deleted;
                                            }
                                        }

                                        if (!exsitingTour.TourHotels[j].TourHotelRoomTypes.Any())
                                            entityContext.Entry(exsitingTour.TourHotels[j]).State = EntityState.Deleted;
                                    }
                                    else
                                    {
                                        for (var i = exsitingTour.TourHotels[j].TourHotelRoomTypes.Count - 1;
                                            i >= 0;
                                            i--)
                                        {
                                            entityContext.Entry(exsitingTour.TourHotels[j].TourHotelRoomTypes[i])
                                                    .State =
                                                EntityState.Deleted;
                                        }

                                        entityContext.Entry(exsitingTour.TourHotels[j]).State = EntityState.Deleted;
                                    }
                                }
                            }
                        }

                        //Handle Customers
                        foreach (var customer in reservation.Customers)
                        {
                            if (customer.CustomerId == 0)
                            {
                                exsitingReservation.Customers.Add(customer);
                            }
                            else
                            {
                                var exsitingCustomer = (from e in exsitingReservation.Customers where e.CustomerId == customer.CustomerId select e).FirstOrDefault();
                                if (exsitingCustomer != null)
                                {
                                    entityContext.Entry(exsitingCustomer).CurrentValues.SetValues(customer);
                                }
                            }
                        }

                        //Delete Customers that are not exist in reservation (The most updated reservation)
                        for (var i = exsitingReservation.Customers.Count - 1; i >= 0; i--)
                        {
                            var exist = reservation.Customers.Exists(n =>
                                n.CustomerId == exsitingReservation.Customers[i].CustomerId);
                            if (!exist)
                            {
                                entityContext.Entry(exsitingReservation.Customers[i]).State = EntityState.Deleted;
                            }
                        }

                        entityContext.Entry(exsitingReservation).OriginalValues["RowVersion"] = reservation.RowVersion;
                        exsitingReservation.UpdateTime = DateTime.Now;
                    }
                    else
                        throw new DbUpdateConcurrencyException();

                    entityContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    log.Error("ERROR! Update Reservation failed. Save was done during edit. ");
                    bRowVersionConflict = true;
                }
                var reservationDB = Get(reservation.ReservationId);
                reservationDB.RowVersionConflict = bRowVersionConflict;
                return reservationDB;
            }
        }

        public Reservation AddReservation(Reservation reservation)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                ResetNavProperties(reservation);
                //Handle Group
                if (reservation.Group != null)
                {
                    var existingGroup = (from e in entityContext.GroupSet where e.ExternalId == reservation.Group.ExternalId select e).FirstOrDefault();
                    if (existingGroup != null)
                    {
                        reservation.Group = null;
                        reservation.GroupID = existingGroup.GroupId;
                    }
                }
                entityContext.Entry(reservation).State = EntityState.Added;
                reservation.UpdateTime = DateTime.Now;
                reservation.CreationTime = DateTime.Now;
                reservation.LockTime = DateTime.Now;
                entityContext.SaveChanges();
            }
            var existingReservation = Get(reservation.ReservationId);
            return existingReservation;
        }

        protected override IQueryable<Reservation> IncludeNavigationProperties(IQueryable<Reservation> query)
        {
            return query
                    .IncludeOptimized(o => o.Agency)
                    .IncludeOptimized(o => o.Agency.Agents)
                    .IncludeOptimized(o => o.Customers)
                    .IncludeOptimized(o => o.Group)
                    .IncludeOptimized(o => o.Tours)
                    .IncludeOptimized(o => o.Operator)
                    .IncludeOptimized(o => o.Tours.Select(t => t.TourType))
                    .IncludeOptimized(o => o.Tours.Select(t => t.TourHotels))
                    .IncludeOptimized(o => o.Tours.Select(t => t.TourOptionals))
                    .IncludeOptimized(o => o.Tours.Select(t => t.SubTours))
                    .IncludeOptimized(o => o.Tours.Select(t => t.TourOptionals.Select(k => k.Optional)))
                    .IncludeOptimized(o => o.Tours.Select(th => th.TourHotels.Select(h => h.Hotel)))
                    .IncludeOptimized(o => o.Tours.Select(th => th.TourHotels.Select(aa => aa.TourHotelRoomTypes)))
                    .IncludeOptimized(o => o.Tours.Select(th => th.TourHotels.Select(aa => aa.TourHotelRoomTypes.Select(bb => bb.HotelRoomType))))
                    .IncludeOptimized(o => o.Tours.Select(th => th.TourHotels.Select(aa => aa.TourHotelRoomTypes.Select(bb => bb.HotelRoomType).Select(cc => cc.RoomType))))
                    .IncludeOptimized(o => o.Tours.Select(th => th.TourHotels.Select(aa => aa.TourHotelRoomTypes.Select(bb => bb.HotelRoomType).Select(cc => cc.HotelRoomTypeDaysRanges))));
        }

        private void ResetNavProperties(Reservation reservation)
        {
            reservation.Tours.ForEach((tour) =>
            {
                tour.TourType = null;
                tour.TourOptionals.ForEach((tourOptional) => { tourOptional.Optional = null; });
                tour.TourHotels.ForEach((tourHotel) =>
                {
                    tourHotel.Hotel = null;
                    tourHotel.TourHotelRoomTypes.ForEach((tourHotelRoomType) => { tourHotelRoomType.HotelRoomType = null; });
                });
            });

            reservation.Operator = null;
            reservation.Agency = null;
            reservation.Agent = null;
        }
    }
}
