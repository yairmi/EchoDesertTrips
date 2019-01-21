using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IRoomTypeRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoomTypeRepository : DataRepositoryBase<RoomType>, IRoomTypeRepository
    {
        protected override DbSet<RoomType> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.RoomTypeSet;
        }

        protected override Expression<Func<RoomType, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.RoomTypeId == id);
        }

        protected override IEnumerable<RoomType> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }
    }
}
