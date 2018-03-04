using EchoDesertTrips.Business.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Business_Engines
{
    [Export(typeof(ICustomerEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerEngine : ICustomerEngine
    {
    }
}
