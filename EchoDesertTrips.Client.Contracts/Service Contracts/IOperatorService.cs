using Core.Common.Contracts;
using Core.Common.ServiceModel;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Client.Contracts
{
    [ServiceContract]
    public interface IOperatorService : IServiceContract
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Operator UpdateOperator(Operator op);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteOperator(Operator op);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Operator GetOperator(string OperatorName, string OperatorPassword);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Operator[] GetAllOperators();
    }
}
