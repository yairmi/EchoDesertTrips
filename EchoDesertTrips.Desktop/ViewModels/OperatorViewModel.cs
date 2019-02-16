using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;
using static Core.Common.Core.Const;

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
            WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
            {
                operatorClient.DeleteOperator(obj);
                Operators.Remove(obj);
            });
        }

        public DelegateCommand<Operator> SaveOperatorCommand { get; set; }

        private Operator LastUpdatedOperator;

        private void OnSaveCommand(Operator op)
        {
            LastUpdatedOperator = op;
            ValidateModel();
            if (op.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                {
                    bool bIsNew = op.OperatorId == 0;
                    var savedOperator = operatorClient.UpdateOperator(op);
                    if (bIsNew)
                        Operators[Operators.Count - 1].OperatorId = savedOperator.OperatorId;

                    NotifyServer("OperatorViewModel OnSaveCommand",
                        SerializeInventoryMessage(eInventoryTypes.E_OPERATOR, eOperation.E_UPDATED, savedOperator.OperatorId), eMsgTypes.E_INVENTORY);
                });
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
