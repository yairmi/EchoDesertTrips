using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Core.Common.Contracts;
using System.Linq.Expressions;
using System;

namespace Core.Common.Data
{
    public abstract class DataRepositoryBase<T, U> : IDataRepository<T>
        where T : class, IIdentifiableEntity, new()
        where U : DbContext, new()
    {
        protected abstract DbSet<T> DbSet(U entityContext);
        protected abstract Expression<Func<T, bool>> IdentifierPredicate(U entityContext, int id);

        protected abstract T AddEntity(U entityContext, T entity);

        protected abstract T UpdateEntity(U entityContext, T entity);

        protected abstract IEnumerable<T> GetEntities(U entityContext);

        protected abstract IEnumerable<T> GetEntities(U entityContext, int id);

        protected abstract T GetEntity(U entityContext, int id);

        public T Add(T entity)
        {
            using (U entityContext = new U())   
            {
                T addedEntity = AddEntity(entityContext, entity);
                entityContext.SaveChanges();
                return addedEntity;
            }
        }

        public void Add(List<T> entities)
        {
            using (U entityContext = new U())
            {
                foreach (var entity in entities)
                {
                    T addedEntity = AddEntity(entityContext, entity);
                }
                entityContext.SaveChanges();
            }
        }

        public void Remove(T entity)
        {
            using (U entityContext = new U())
            {
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public void Remove(int id)
        {
            using (U entityContext = new U())
            {
                T entity = GetEntity(entityContext, id);
                entityContext.Entry<T>(entity).State = EntityState.Deleted;
                entityContext.SaveChanges();
            }
        }

        public void RemoveAll(int id)
        {
            using (U entityContext = new U())
            {
                var entities = GetEntities(entityContext, id);
                foreach(var entity in entities)
                {
                    entityContext.Entry<T>(entity).State = EntityState.Deleted;
                }
                entityContext.SaveChanges();
            }
        }

        public T Update(T entity)
        {
            using (U entityContext = new U())
            {
                //Get exsiting entity according to incomming entity (by Id)
                T existingEntity = UpdateEntity(entityContext, entity);
                //Set the values of the incomming entity to existingEntity. (Navigation properties are not updated)
                entityContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                //Load navigation property into existing entity
                //LoadNavigationProperties(entityContext, existingEntity ,entity);
                entityContext.SaveChanges();
                return existingEntity;
            }
        }

        public IEnumerable<T> Get()
        {
            using (U entityContext = new U())
                return (GetEntities(entityContext)).ToArray().ToList();
        }

        public T Get(int id)
        {
            using (U entityContext = new U())
                return GetEntity(entityContext, id);
        }
    }
}
