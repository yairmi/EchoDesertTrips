using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
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
    public class OptionalsViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;

        [ImportingConstructor]
        public OptionalsViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            DeleteOptionalCommand = new DelegateCommand<Optional>(OnDeleteCommand);
            SaveOptionalCommand = new DelegateCommand<Optional>(OnSaveCommand);
            RowEditEndingCommand = new DelegateCommand<Optional>(OnRowEditEndingCommand);
        }

        public DelegateCommand<Optional> DeleteOptionalCommand { get; set; }

        private void OnDeleteCommand(Optional obj)
        {
            WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                inventoryClient.DeleteOptional(obj);
                Inventories.Optionals.Remove(obj);
            });
        }

        public DelegateCommand<Optional> SaveOptionalCommand { get; set; }

        private Optional LastUpdatedOptional;

        private void OnSaveCommand(Optional optional)
        {
            LastUpdatedOptional = optional;
            ValidateModel();
            if (optional.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = optional.OptionalId == 0;
                    var savedOptional = inventoryClient.UpdateOptional(optional);
                    if (bIsNew)
                        Inventories.Optionals[Inventories.Optionals.Count - 1].OptionalId = savedOptional.OptionalId;
                    try
                    {
                        Client.NotifyServer(
                            SerializeInventoryMessage(eInventoryTypes.E_OPTIONAL, eOperation.E_UPDATED, savedOptional.OptionalId), eMsgTypes.E_INVENTORY);
                    }
                    catch(Exception ex)
                    {
                        log.Error("Notify Server Error: " + ex.Message);
                    }
                });

                
            }
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(LastUpdatedOptional);
        }

        public DelegateCommand<Optional> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(Optional optional)
        {
            if (optional.IsDirty)
                OnSaveCommand(optional);
        }

        private void OptionalUpdated(OptionalEventArgs e)
        {
            Inventories.Update(e.Optional);
        }

        public override string ViewTitle => "Optionals";
    }

    public class OptionalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            Optional optional = (value as BindingGroup).Items[0] as Optional;
            if (optional.OptionalDescription == string.Empty)
            {
                return new ValidationResult(false,
                    "Optional Description should not be empty");
            }
            else
                if (optional.PricePerPerson < 0)
            {
                return new ValidationResult(false,
                    "Price Per Person must be equal or greater then 0");
            }
            else
                if (optional.PriceInclusive < 0)
            {
                return new ValidationResult(false,
                    "Price Inclusive must be equal or greater then 0");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }
}
