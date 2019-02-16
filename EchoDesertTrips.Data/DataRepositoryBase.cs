using Core.Common.Contracts;
using Core.Common.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace EchoDesertTrips.Data
{
    public class DataRepositoryBase<T> : DataRepositoryBase<T, EchoDesertTripsContext>
        where T : class, IIdentifiableEntity, new()
    {
        protected override DbSet<T> DbSet(EchoDesertTripsContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override Expression<Func<T, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            throw new NotImplementedException();
        }

        protected override T AddEntity(EchoDesertTripsContext entityContext, T entity)
        {
            return DbSet(entityContext).Add(entity);
        }

        protected override IEnumerable<T> GetEntities(EchoDesertTripsContext entityContext)
        {
            var query = (from e in DbSet(entityContext) select e);
            return IncludeNavigationProperties(query);
        }

        protected override IEnumerable<T> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            throw new NotImplementedException();
        }

        protected override T GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = DbSet(entityContext).Where(IdentifierPredicate(entityContext, id));
            return IncludeNavigationProperties(query).FirstOrDefault();
        }

        protected override T UpdateEntity(EchoDesertTripsContext entityContext, T entity)
        {
            var q = DbSet(entityContext).Where(IdentifierPredicate(entityContext, entity.EntityId));
            return q.FirstOrDefault();
        }

        protected override IQueryable<T> IncludeNavigationProperties(IQueryable<T> query)
        {
            return query;
        }

        protected readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
