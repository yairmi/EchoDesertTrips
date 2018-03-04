using Core.Common.Exceptions;
using EchoDesertTrips.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EchoDesertTrips.Business.Contracts
{
    [ServiceContract]
    public interface IOperatorService
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
