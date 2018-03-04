using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(INationalityRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NationalityRepository : DataRepositoryBase<Nationality>, INationalityRepository
    {
        protected override Nationality AddEntity(EchoDesertTripsContext entityContext, Nationality entity)
        {
            return entityContext.NationalitySet.Add(entity);
        }

        protected override Nationality UpdateEntity(EchoDesertTripsContext entityContext, Nationality entity)
        {
            return (from e in entityContext.NationalitySet where e.NationalityId == entity.NationalityId select e).FirstOrDefault();
        }

        protected override IEnumerable<Nationality> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.NationalitySet select e);
        }

        protected override IEnumerable<Nationality> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Nationality GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.NationalitySet where e.NationalityId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }

        protected override void LoadNavigationProperties(EchoDesertTripsContext entityContext, Nationality existingEntity, Nationality entity)
        {
        }
    }
}
