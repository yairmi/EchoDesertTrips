using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(IAgencyRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgencyRepository : DataRepositoryBase<Agency>, IAgencyRepository
    {
        protected override Agency AddEntity(EchoDesertTripsContext entityContext, Agency entity)
        {
            return entityContext.AgencySet.Add(entity);
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

        protected override Agency UpdateEntity(EchoDesertTripsContext entityContext, Agency entity)
        {
            //return (from e in entityContext.AgencySet where e.AgencyId == entity.AgencyId select e).FirstOrDefault();
            return (from e in entityContext.AgencySet where e.AgencyId == entity.AgencyId select e).FirstOrDefault();


        }

        protected override IEnumerable<Agency> GetEntities(EchoDesertTripsContext entityContext)
        {
            return entityContext.AgencySet.Include(a => a.Agents).ToList();
        }

        protected override IEnumerable<Agency> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override Agency GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            var query = (from e in entityContext.AgencySet where e.AgencyId == id select e);
            var results = query.FirstOrDefault();

            return results;

        }
   }
}
