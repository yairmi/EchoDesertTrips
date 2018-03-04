using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OptionalsViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;

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
                Optionals.Remove(obj);
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
                        Optionals[Optionals.Count - 1].OptionalId = savedOptional.OptionalId;
                });

                NotifyServer("OptionalsViewModel OnSaveCommand", 2);
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

        public override string ViewTitle => "Optionals";

        //protected override void OnViewLoaded()
        //{
        //    try
        //    {
        //        WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
        //        {
        //            Optionals.Clear();
        //            Optional[] optionals = inventoryClient.GetAllOptionals();
        //            foreach (var optional in optionals)
        //                Optionals.Add(optional);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("Exception load optionals: " + ex.Message);
        //    }
        //}
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
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }
}
