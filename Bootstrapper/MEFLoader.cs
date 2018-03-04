using EchoDesertTrips.Business.Business_Engines;
using EchoDesertTrips.Data;
using EchoDesertTrips.Data.Data_Repositories;
using System.ComponentModel.Composition.Hosting;

namespace Bootstrapper
{
    public static class MEFLoader
    {
        public static CompositionContainer Init()
        {
            AggregateCatalog catalog = new AggregateCatalog();
                
            //Add Items to catalog here
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(CustomerRepository).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(TourEngine).Assembly));

            CompositionContainer container = new CompositionContainer(catalog);

            return container;
        }
    }
}
