using Core.Common.Exceptions;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
                log.Error(string.Empty);
                throw ex;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                log.Error(string.Empty);
                var updateEx = new UpdateConcurrencyException(ex.Message, ex);
                throw new FaultException<UpdateConcurrencyException>(updateEx, updateEx.Message);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                log.Error(string.Empty);
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
                log.Error(string.Empty);
                throw ex;
            }
            catch (Exception ex)
            {
                log.Error(string.Empty);
                throw new FaultException(ex.Message);
            }
        }

        protected readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
