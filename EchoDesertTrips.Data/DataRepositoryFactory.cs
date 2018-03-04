using Core.Common.Contracts;
using Core.Common.Core;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Data
{
    //The job of DataRepositoryFactory is to go and find me data repository
    //T must be of type IDataRepository
    [Export(typeof(IDataRepositoryFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        //GetExportedValue<T> is MEF resolve. this is how to resolve something manually through MEF
        T IDataRepositoryFactory.GetDataRepository<T>()
        {
            return ObjectBase.Container.GetExportedValue<T>();
        }
    }
}
