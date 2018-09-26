using Core.Common.Contracts;
using Core.Common.Data;
using System;
using System.Collections.Generic;

namespace EchoDesertTrips.Data
{
    public class DataRepositoryBase<T> : DataRepositoryBase<T, EchoDesertTripsContext>
        where T : class, IIdentifiableEntity, new()
    {
        protected override T AddEntity(EchoDesertTripsContext entityContext, T entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<T> GetEntities(EchoDesertTripsContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<T> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            throw new NotImplementedException();
        }

        protected override T GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            throw new NotImplementedException();
        }

        protected override T UpdateEntity(EchoDesertTripsContext entityContext, T entity)
        {
            throw new NotImplementedException();
        }

        protected readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
