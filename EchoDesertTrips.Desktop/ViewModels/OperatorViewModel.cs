using Core.Common.Contracts;
using Core.Common.UI.Core;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;
using static Core.Common.Core.Const;
using Core.Common.UI.CustomEventArgs;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OperatorViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;

        [ImportingConstructor]
        public OperatorViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            DeleteOperatorCommand = new DelegateCommand<Operator>(DeleteCommand);
            SaveOperatorCommand = new DelegateCommand<Operator>(OnSaveCommand);
            RowEditEndingCommand = new DelegateCommand<Operator>(OnRowEditEndingCommand);
        }

        public DelegateCommand<Operator> DeleteOperatorCommand { get; set; }

        private void DeleteCommand(Operator obj)
        {
            try
            {
                WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                {
                    operatorClient.DeleteOperator(obj);
                    Inventories.Operators.Remove(obj);
                });
            }
            catch(Exception ex)
            {
                log.Error(string.Empty, ex);
            }

            
        }

        public DelegateCommand<Operator> SaveOperatorCommand { get; set; }

        private void OnSaveCommand(Operator op)
        {
            ValidateModel();
            if (op.IsValid)
            {
                try
                {
                    WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                    {
                        bool bIsNew = op.OperatorId == 0;
                        var savedOperator = operatorClient.UpdateOperator(op);

                        if (bIsNew)
                            Inventories.Operators[Inventories.Operators.Count - 1].OperatorId = savedOperator.OperatorId;
                        else
                        {
                            CurrentOperator.UpdateOperator(savedOperator);
                        }

                        try
                        {
                            Client.NotifyServer(
                                SerializeInventoryMessage(eInventoryTypes.E_OPERATOR, eOperation.E_UPDATED, savedOperator.OperatorId), eMsgTypes.E_INVENTORY);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    });
                }
                catch(Exception ex)
                {
                    log.Error(string.Empty, ex);
                }
            }
        }

        public DelegateCommand<Operator> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(Operator op)
        {
            if (op.IsDirty)
                OnSaveCommand(op);
        }

        public override string ViewTitle => "Operators";
    }

    public class OperatorValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            Operator op = (value as BindingGroup).Items[0] as Operator;
            if (op.OperatorName == string.Empty)
            {
                return new ValidationResult(false,
                    "Operator name should not be empty");
            }
            else if (op.Password == string.Empty)
            {
                return new ValidationResult(false,
                    "Operator Password should not be empty");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }
}
