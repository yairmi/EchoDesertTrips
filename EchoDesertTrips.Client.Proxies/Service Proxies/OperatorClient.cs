using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Client.Proxies.Service_Proxies
{
    [Export(typeof(IOperatorService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class OperatorClient : UserClientBase<IOperatorService>, IOperatorService
    {
        public Operator UpdateOperator(Operator op)
        {
            return Channel.UpdateOperator(op);
        }

        public void DeleteOperator(Operator op)
        {
            Channel.DeleteOperator(op);
        }

        public Operator GetOperator(string OperatorName, string OperatorPassword)
        {
            return Channel.GetOperator(OperatorName, OperatorPassword);
        }

        public Operator[] GetAllOperators()
        {
            return Channel.GetAllOperators();
        }
    }
}
