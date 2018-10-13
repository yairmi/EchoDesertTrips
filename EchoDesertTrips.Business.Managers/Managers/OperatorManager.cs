using EchoDesertTrips.Business.Contracts;
using System;
using EchoDesertTrips.Business.Entities;
using Core.Common.Core;
using System.ComponentModel.Composition;
using Core.Common.Contracts;
using EchoDesertTrips.Data.Contracts.Repository_Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace EchoDesertTrips.Business.Managers.Managers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple,
    ReleaseServiceInstanceOnTransactionComplete = false)]
    public class OperatorManager : ManagerBase, IOperatorService
    {
        public OperatorManager()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        public OperatorManager(IDataRepositoryFactory dataRepositoryFactory)
        {
            _DataRepositoryFactory = dataRepositoryFactory;
        }

        [Import]
        private IDataRepositoryFactory _DataRepositoryFactory;

        [OperationBehavior(TransactionScopeRequired = true)]
        public Operator UpdateOperator(Operator op)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IOperatorRepository operatorRepository =
                    _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();

                Operator updateEntity = null;

                if (op.OperatorId == 0)
                {
                    updateEntity = operatorRepository.Add(op);
                }
                else
                {
                    updateEntity = operatorRepository.Update(op);
                }

                return updateEntity;
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void DeleteOperator(Operator op)
        {
            ExecuteFaultHandledOperation(() =>
            {
                IOperatorRepository operatorRepository =
                    _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();

                operatorRepository.Remove(op.OperatorId);
            });
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public Operator GetOperator(string OperatorName, string OperatorPassword)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IOperatorRepository operatorRepository =
                    _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();

                Operator op = operatorRepository.GetOperator(OperatorName, OperatorPassword);
                return op;
            });
        }
        [OperationBehavior(TransactionScopeRequired = true)]
        public Operator[] GetAllOperators()
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IOperatorRepository operatorRepository =
                    _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();

                IEnumerable<Operator> Operators = operatorRepository.Get();
                return Operators.ToArray();
            });
        }

        public Operator GetOperatorByID(int OperatorID)
        {
            return ExecuteFaultHandledOperation(() =>
            {
                IOperatorRepository operatorRepository =
                    _DataRepositoryFactory.GetDataRepository<IOperatorRepository>();

                Operator op = operatorRepository.Get(OperatorID);
                return op;
            });
        }
    }
}
