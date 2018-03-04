using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Contracts
{
    public interface IDataRepository
    {

    }

    public interface IDataRepository<T> : IDataRepository
        where T : class, IIdentifiableEntity, new()
    {
        T Add(T entity);

        void Add(List<T> entities);

        void Remove(T entity);

        void Remove(int id);

        void RemoveAll(int id);

        T Update(T entity);

        IEnumerable<T> Get();

        T Get(int id);
    }
}
