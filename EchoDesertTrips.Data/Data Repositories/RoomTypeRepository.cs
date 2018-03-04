using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IRoomTypeRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoomTypeRepository : DataRepositoryBase<RoomType>, IRoomTypeRepository
    {
        protected override RoomType AddEntity(EchoDesertTripsContext entityContext, RoomType entity)
        {
            return entityContext.RoomTypeSet.Add(entity);
        }

        protected override RoomType UpdateEntity(EchoDesertTripsContext entityContext, RoomType entity)
        {
            return (from e in entityContext.RoomTypeSet where e.RoomTypeId == entity.RoomTypeId select e).FirstOrDefault();
        }

        protected override IEnumerable<RoomType> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.RoomTypeSet select e);
        }

        protected override IEnumerable<RoomType> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override RoomType GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.RoomTypeSet where e.RoomTypeId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }
    }
}
