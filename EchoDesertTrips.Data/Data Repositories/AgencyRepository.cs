using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System;

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
                             .Include(a => a.Agents)
                             .FirstOrDefault();
                entityContext.Entry(exsitingAgency).CurrentValues.SetValues(agency);
                //Agents
                if (agency.Agents != null)
                    foreach (var agent in agency.Agents)
                    {
                        var existingAgent= (from e in entityContext.AgentSet
                                                    where e.AgentId == agent.AgentId
                                                    select e).FirstOrDefault();
                        if (existingAgent != null)
                        {
                            entityContext.Entry(existingAgent).CurrentValues.SetValues(agent);
                        }
                        else //new Agent
                        {
                            exsitingAgency.Agents.Add(agent);
                        }
                    }
                entityContext.SaveChanges();

                return exsitingAgency;
            }
        }

        protected override IEnumerable<Agency> GetEntities(EchoDesertTripsContext entityContext)
        {
            return entityContext.AgencySet.Include(a => a.Agents);
        }

        protected override IEnumerable<Agency> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }
   }
}
