﻿using EchoDesertTrips.Business.Entities;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Data.Entity;
using System.Linq.Expressions;
using System;

namespace EchoDesertTrips.Data.Data_Repositories
{
    [Export(typeof(ITourTypeRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourTypeRepository : DataRepositoryBase<TourType>, ITourTypeRepository
    {
        protected override DbSet<TourType> DbSet(EchoDesertTripsContext entityContext)
        {
            return entityContext.TourTypeSet;
        }

        protected override Expression<Func<TourType, bool>> IdentifierPredicate(EchoDesertTripsContext entityContext, int id)
        {
            return (e => e.TourTypeId == id);
        }

        protected override IEnumerable<TourType> GetEntities(EchoDesertTripsContext entityContext)
        {
            return (from e in entityContext.TourTypeSet select e).Include(t => t.TourTypeDescriptions);
        }

        protected override IEnumerable<TourType> GetEntities(EchoDesertTripsContext entityContext, int id)
        {
            return null;
        }

        protected override TourType GetEntity(EchoDesertTripsContext entityContext, int id)
        {
            return  (from e in entityContext.TourTypeSet where e.TourTypeId == id select e)
                .Include(t => t.TourTypeDescriptions).FirstOrDefault();
        }

        public TourType UpdateTourType(TourType tourType)
        {
            using (EchoDesertTripsContext entityContext = new EchoDesertTripsContext())
            {
                var exsitingTourType = (from e in entityContext.TourTypeSet where e.TourTypeId == tourType.TourTypeId select e)
                             .Include(h => h.TourTypeDescriptions).FirstOrDefault();
                entityContext.Entry(exsitingTourType).CurrentValues.SetValues(tourType);
                if (tourType.TourTypeDescriptions != null)
                {
                    foreach (var tourTypeDescription in tourType.TourTypeDescriptions)
                    {
                        if (tourTypeDescription.TourTypeDescriptionId == 0)
                        {
                            exsitingTourType.TourTypeDescriptions.Add(tourTypeDescription);
                        }
                        else
                        {
                            var existingTourDescription = (from e in exsitingTourType.TourTypeDescriptions where e.TourTypeDescriptionId == tourTypeDescription.TourTypeDescriptionId select e).FirstOrDefault();
                            if (existingTourDescription != null)
                            {
                                entityContext.Entry(existingTourDescription).CurrentValues.SetValues(tourTypeDescription);
                            }
                            else
                            {
                                log.Error("Fail to add tourTypeDescription.TourTypeDescriptionId: " + tourTypeDescription.TourTypeDescriptionId);
                            }
                        }
                    }
                }
                entityContext.SaveChanges();

                return exsitingTourType;
            }
        }
    }
}
