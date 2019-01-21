﻿using EchoDesertTrips.Business.Contracts;
using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.DTOs;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IHotelRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotelRepository : DataRepositoryBase<Hotel>, IHotelRepository
    {
        protected override DbSet<Hotel> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.HotelSet;
        }

        protected override Expression<Func<Hotel, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.HotelId == id);
        }

        protected override Hotel GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            return (from e in entityContext.HotelSet where e.HotelId == id select e)
                .Include(a => a.HotelRoomTypes.Select(r => r.RoomType)).FirstOrDefault();
        }

        public Hotel AddHotel(Hotel entity)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                entity.HotelRoomTypes.ForEach((hotelRoomType) =>
                {
                    hotelRoomType.RoomType = null;
                });

                var hotel = entityContext.HotelSet.Add(entity);
                entityContext.SaveChanges();
                if (hotel != null)
                {
                    return GetEntity(entityContext, hotel.HotelId);
                }
                return null;
            }
        }

        public Hotel UpdateHotel(Hotel hotel)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var exsitingHotel = (from e in entityContext.HotelSet where e.HotelId == hotel.HotelId select e)
                             .Include(h => h.HotelRoomTypes.Select(r => r.RoomType))
                             .FirstOrDefault();
                entityContext.Entry(exsitingHotel).CurrentValues.SetValues(hotel);
                //Hotel Room Types
                if (hotel.HotelRoomTypes != null)
                {
                    foreach (var hotelRoomType in hotel.HotelRoomTypes)
                    {
                        if (hotelRoomType.HotelRoomTypeId == 0)
                        {
                            hotelRoomType.RoomType = null;
                            entityContext.HotelRoomTypeSet.Add(hotelRoomType);
                        }
                        else
                        {
                            var existingHotelRoomType = (from e in exsitingHotel.HotelRoomTypes where e.HotelRoomTypeId == hotelRoomType.HotelRoomTypeId select e).FirstOrDefault();
                            if (existingHotelRoomType != null)
                            {
                                entityContext.Entry(existingHotelRoomType).CurrentValues.SetValues(hotelRoomType);
                            }
                        }
                    }
                }
                entityContext.SaveChanges();
                if (exsitingHotel != null)
                {
                    return GetEntity(entityContext, hotel.HotelId);
                }
                return null;
            }
        }

        protected override IEnumerable<Hotel> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.HotelSet select e)
                .Include(h => h.HotelRoomTypes.Select(r => r.RoomType));
        }

        protected override IEnumerable<Hotel> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return (from e in entityContext.HotelSet select e)
                .Where(h => h.HotelId == id)
                .Include(h => h.HotelRoomTypes.Select(r => r.RoomType));
        }
    }
}
