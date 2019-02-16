using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System;
using Z.EntityFramework.Plus;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IAgencyRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgencyRepository : DataRepositoryBase<Agency>, IAgencyRepository
    {
        protected override DbSet<Agency> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.AgencySet;
        }

        protected override Expression<Func<Agency, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.AgencyId == id);
        }

        public Agency UpdateAgency(Agency agency)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var exsitingAgency = (from e in entityContext.AgencySet where e.AgencyId == agency.AgencyId select e)
                             .IncludeOptimized(a => a.Agents)
                             .FirstOrDefault();
                entityContext.Entry(exsitingAgency).CurrentValues.SetValues(agency);
                //Agents
                if (agency.Agents != null)
                {
                    foreach (var agent in agency.Agents)
                    {
                        if (agent.AgentId == 0)
                        {
                            exsitingAgency.Agents.Add(agent);
                        }
                        else
                        {
                            var existingAgent = (from e in exsitingAgency.Agents where e.AgentId == agent.AgentId select e).FirstOrDefault();
                            if (existingAgent != null)
                            {
                                entityContext.Entry(existingAgent).CurrentValues.SetValues(agent);
                            }
                            else
                            {
                                log.Error("Fail to update agent.AgentId: " + agent.AgentId);
                            }
                        }
                    }
                }
                entityContext.SaveChanges();

                return exsitingAgency;
            }
        }

        protected override IEnumerable<Agency> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override IQueryable<Agency> IncludeNavigationProperties(IQueryable<Agency> query)
        {
            return query.IncludeOptimized(a => a.Agents);
        }
    }
}
