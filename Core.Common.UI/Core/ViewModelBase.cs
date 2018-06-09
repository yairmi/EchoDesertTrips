using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Core;
using FluentValidation.Results;
using EchoDesertTrips.Client.Entities;
using System.Collections.ObjectModel;
using EchoDesertTrips.Client.Proxies.Service_Proxies;
using EchoDesertTrips.Client.Contracts;

namespace Core.Common.UI.Core
{
    public class ViewModelBase : ObjectBase
    {
        public ViewModelBase()
        {
            ToggleErrorsCommand = new DelegateCommand<object>(OnToggleErrorsCommandExecute, OnToggleErrorsCommandCanExecute);
            _optionals = new ObservableCollection<Optional>();
            _tourTypes = new ObservableCollection<TourTypeWrapper>();
            _hotels = new ObservableCollection<Hotel>();
            _agencies = new ObservableCollection<Agency>();
            _roomTypes = new ObservableCollection<RoomType>();
        }

        bool _ErrorsVisible = false;

        public object ViewLoaded
        {
            get
            {
                OnViewLoaded();
                return null;
            }
        }

        protected virtual void OnViewLoaded() { }

        protected void WithClient<T>(T proxy, Action<T> codeToExecute)
        {
            codeToExecute.Invoke(proxy);

            var disposableClient = proxy as IDisposable;
            disposableClient?.Dispose();
        }

        public virtual string ViewTitle => string.Empty;

        List<ObjectBase> _models;

        protected virtual void AddModels(List<ObjectBase> models) { }

        protected void ValidateModel(bool bClear = false)
        {
            if (bClear == true && _models != null)
            {
                _models.Clear();
                _models = null;
            }
            if (_models == null)
            {
                _models = new List<ObjectBase>();
                AddModels(_models);
            }

            _ValidationErrors = new List<ValidationFailure>();

            if (_models.Count > 0)
            {
                foreach (ObjectBase modelObject in _models)
                {
                    modelObject?.Validate();

                    _ValidationErrors = _ValidationErrors.Union(modelObject.ValidationErrors).ToList();
                }

                OnPropertyChanged(() => ValidationErrors, false);
                OnPropertyChanged(() => ValidationHeaderText, false);
                OnPropertyChanged(() => ValidationHeaderVisible, false);
            }
        }

        public DelegateCommand<object> ToggleErrorsCommand { get; protected set; }

        public virtual bool ValidationHeaderVisible => ValidationErrors != null && ValidationErrors.Any();

        public virtual bool ErrorsVisible
        {
            get { return _ErrorsVisible; }
            set
            {
                if (_ErrorsVisible == value)
                    return;

                _ErrorsVisible = value;
                OnPropertyChanged(() => ErrorsVisible, false);
            }
        }

        public virtual string ValidationHeaderText
        {
            get
            {
                var ret = string.Empty;

                if (ValidationErrors != null)
                {
                    var verb = (ValidationErrors.Count() == 1 ? "is" : "are");
                    var suffix = (ValidationErrors.Count() == 1 ? "" : "s");

                    if (!IsValid)
                        ret = $"There {verb} {ValidationErrors.Count()} validation error{suffix}.";
                }

                return ret;
            }
        }

        private Operator _operator;

        public Operator Operator
        {
            get
            {
                return _operator;
            }
            set
            {
                _operator = value;
                OnPropertyChanged(() => Operator);
            }
        }

        private ObservableCollection<Hotel> _hotels;

        public ObservableCollection<Hotel> Hotels
        {
            get
            {
                return _hotels;
            }

            set
            {
                _hotels = value;
                OnPropertyChanged(() => Hotels, false);
            }
        }

        private ObservableCollection<TourTypeWrapper> _tourTypes;

        public ObservableCollection<TourTypeWrapper> TourTypes
        {
            get
            {
                return _tourTypes;
            }

            set
            {
                _tourTypes = value;
                OnPropertyChanged(() => TourTypes, false);
            }
        }

        private ObservableCollection<Optional> _optionals;

        public ObservableCollection<Optional> Optionals
        {
            get
            {
                return _optionals;
            }

            set
            {
                _optionals = value;
                OnPropertyChanged(() => Optionals, false);
            }
        }

        private ObservableCollection<RoomType> _roomTypes;

        public ObservableCollection<RoomType> RoomTypes
        {
            get
            {
                return _roomTypes;
            }
            set
            {
                _roomTypes = value;
                OnPropertyChanged(() => RoomTypes);
            }
        }

        private ObservableCollection<Agency> _agencies;

        public ObservableCollection<Agency> Agencies
        {
            get
            {
                return _agencies;
            }

            set
            {
                _agencies = value;
                OnPropertyChanged(() => Agencies, false);
            }
        }

        protected virtual void OnToggleErrorsCommandExecute(object arg)
        {
            ErrorsVisible = !ErrorsVisible;
        }

        protected virtual bool OnToggleErrorsCommandCanExecute(object arg)
        {
            return !IsValid;
        }

        public BroadcastorServiceClient Client;

        protected void NotifyServer(string calledFrom, int messageId)
        {
            try
            {
                Client.NotifyServer(new EventDataType()
                {
                    ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
                    EventMessage = messageId.ToString()
                });
            }
            catch (Exception ex)
            {
                log.Error(calledFrom + ": Failed to notify server" + ex.Message);
            }

        }

        protected void InitTourOptionals(TourWrapper tour)
        {
            var tourOptionals = new ObservableCollection<TourOptionalWrapper>();
            foreach (var optional in Optionals)
            {
                var tourOptional = tour.TourOptionals.FirstOrDefault(o => o.OptionalId == optional.OptionalId);
                if (tourOptional == null)
                {
                    var newTourOptional = new TourOptionalWrapper()
                    {
                        Selected = false,
                        Optional = optional,
                        OptionalId = optional.OptionalId,
                        TourId = tour.TourId,
                        PriceInclusive = false
                    };
                    tourOptionals.Add(newTourOptional);
                }
                else
                {
                    tourOptional.Selected = true;
                    tourOptionals.Add(tourOptional);
                }
            }
            tour.TourOptionals.Clear();
            tour.TourOptionals = tourOptionals;
        }

        protected int GetCustomerLeft(ReservationWrapper reservation)
        {
            var customersInHotels = 0;
            reservation.Tours?.ToList().ForEach((tour) =>
            {
                tour.TourHotels?.ToList().ForEach((tourHotel) =>
                {
                    tourHotel.TourHotelRoomTypes?.ToList().ForEach((hotelRoomType) =>
                    {
                        customersInHotels += (hotelRoomType.Persons);
                    });
                });
            });

            return Math.Max(reservation.NumberOfCustomers, customersInHotels) - reservation.Customers.Count;
        }

        protected readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
