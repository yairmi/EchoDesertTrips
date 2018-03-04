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
    [Export(typeof(IOperatorRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class OperatorRepository : DataRepositoryBase<Operator>, IOperatorRepository
    {
        protected override Operator AddEntity(EchoDesertTripsContext entityContext, Operator entity)
        {
            return entityContext.OperatorSet.Add(entity);
        }

        protected override Operator UpdateEntity(EchoDesertTripsContext entityContext, Operator entity)
        {
            return (from e in entityContext.OperatorSet where e.OperatorId == entity.OperatorId select e).FirstOrDefault();
        }

        protected override IEnumerable<Operator> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.OperatorSet select e);
        }

        protected override IEnumerable<Operator> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Operator GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.OperatorSet where e.OperatorId == id select e);
            var results = query.FirstOrDefault();

            return results;
        }

        public Operator GetOperator(string OperatorName, string OperatorPassword)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var result = (from e in entityContext.OperatorSet
                                where e.OperatorName == OperatorName &&
                                e.Password == OperatorPassword
                                select e).FirstOrDefault();
                return result;
            }
        }
    }
}
