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
    [Export(typeof(IAgentRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgentRepository : DataRepositoryBase<Agent>, IAgentRepository
    {
        protected override DbSet<Agent> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.AgentSet;
        }

        protected override Expression<Func<Agent, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.AgentId == id);
        }

        protected override IEnumerable<Agent> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }
    }
}
