using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.Windows.Controls;
using System.Windows.Data;
using Core.Common.Core;
using System.Collections.Generic;
using System.Linq;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NationalityViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;

        [ImportingConstructor]
        public NationalityViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            DaleteNationalityCommand = new DelegateCommand<Nationality>(OnDeleteCommand);
            SaveNationalityCommand = new DelegateCommand<Nationality>(OnSaveCommand);
            RowEditEndingCommand = new DelegateCommand<Nationality>(OnRowEditEndingCommand);
        }

        public DelegateCommand<Nationality> DaleteNationalityCommand { get; set; }

        private void OnDeleteCommand(Nationality obj)
        {
            WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                inventoryClient.DeleteNationality(obj);
                Nationalities.Remove(obj);
            });
        }
    
        
        public DelegateCommand<Nationality> SaveNationalityCommand { get; set; }

        private Nationality LastUpdatedNationality;

        private void OnSaveCommand(Nationality nationality)
        {
            LastUpdatedNationality = nationality;
            ValidateModel();
            if (nationality.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = nationality.NationalityId == 0;
                    var savedNationality = inventoryClient.UpdateNationality(nationality);
                    if (bIsNew)
                        Nationalities[Nationalities.Count - 1].NationalityId = savedNationality.NationalityId;

                });
            }
            else
            {
                //if (nationality.NationalityId == 0) //If it is new nationality
                //    Nationalities.RemoveAt(Nationalities.Count - 1);
            }
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(LastUpdatedNationality);
        }

        public DelegateCommand<Nationality> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(Nationality nationality)
        {
            if (nationality.IsDirty)
                OnSaveCommand(nationality);
        }

        public override string ViewTitle => "Nationalities";

        //private ObservableCollection<Nationality> _nationalities;

        //public ObservableCollection<Nationality> Nationalities
        //{
        //    get
        //    {
        //        return _nationalities;
        //    }

        //    set
        //    {
        //        _nationalities = value;
        //        OnPropertyChanged(() => Nationalities, false);
        //    }
        //}

        protected override void OnViewLoaded()
        {
            try
            {
                WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    var nationalities = inventoryClient.GetAllNationalities();
                    Nationalities = new ObservableCollection</*NationalityWrapper*/Nationality>(nationalities);
                });
            }
            catch (Exception ex)
            {
                log.Error("Exception load nationalities: " + ex.Message);
            }
        }
    }

    public class NationalityValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            Nationality nationality = (value as BindingGroup).Items[0] as Nationality;
            if (nationality.NationalityName == string.Empty)
            {
                return new ValidationResult(false,
                    "Nationality name should not be empty");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }
}
