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
    [Export(typeof(IAgentRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgentRepository : DataRepositoryBase<Agent>, IAgentRepository
    {
        protected override Agent AddEntity(EchoDesertTripsContext entityContext, Agent entity)
        {
            return entityContext.AgentSet.Add(entity);
        }

        protected override Agent UpdateEntity(EchoDesertTripsContext entityContext, Agent entity)
        {
            return (from e in entityContext.AgentSet where e.AgentId == entity.AgentId select e).FirstOrDefault();
        }

        protected override IEnumerable<Agent> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.AgentSet select e);
        }

        protected override IEnumerable<Agent> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Agent GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.AgentSet where e.AgentId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }
    }
}
