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
    [Export(typeof(IOperatorRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class OperatorRepository : DataRepositoryBase<Operator>, IOperatorRepository
    {
        protected override DbSet<Operator> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.OperatorSet;
        }

        protected override Expression<Func<Operator, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.OperatorId == id);
        }

        protected override IEnumerable<Operator> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
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
