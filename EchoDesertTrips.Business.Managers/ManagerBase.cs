using Core.Common.Exceptions;
using System;
using System.Data.Entity.Infrastructure;
using System.ServiceModel;

namespace EchoDesertTrips.Business.Managers
{
    public class ManagerBase
    {
        public ManagerBase()
        {
            OperationContext context = OperationContext.Current;

            //This line tells MEF to resolve the dependencies for this class after the class
            //has been constructed.
        }

        protected T ExecuteFaultHandledOperation<T>(Func<T> CodeToExecute)
        {
            try
            {
                return CodeToExecute.Invoke();
            }
            catch (FaultException ex)
            {
                log.Error("FaultException: " + ex.Message);
                throw ex;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                log.Error("DbUpdateConcurrencyException: " + ex.Message);
                var updateEx = new UpdateConcurrencyException(ex.Message, ex);
                throw new FaultException<UpdateConcurrencyException>(updateEx, updateEx.Message);
            }
            catch (Exception ex)
            {
                log.Error("Exception: " + ex.Message);
                throw new FaultException(ex.Message);
            }
        }

        protected void ExecuteFaultHandledOperation(Action CodeToExecute)
        {
            try
            {
                CodeToExecute.Invoke();
            }
            catch (FaultException ex)
            {
                log.Error("FaultException: " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error("Exception: " + ex.Message);
                throw new FaultException(ex.Message);
            }
        }

        protected readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
