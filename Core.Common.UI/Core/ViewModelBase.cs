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
            _tourTypes = new ObservableCollection<TourType>();
            _hotels = new ObservableCollection<Hotel>();
            _agencies = new RangeObservableCollection<Agency>();
            _roomTypes = new RangeObservableCollection<RoomType>();
            _operators = new RangeObservableCollection<Operator>();
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

        protected void WithClient<T>(T proxy, Action<T> codeToExecute, string methodName = "", int position = 0)
        {
            try
            {
                codeToExecute.Invoke(proxy);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Method Name {0} failed in postion {1}. Exception: {2}", methodName, position, e.Message));
            }
            finally
            {
                var disposableClient = proxy as IDisposable;
                disposableClient?.Dispose();
            }
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

        private ObservableCollection<TourType> _tourTypes;

        public ObservableCollection<TourType> TourTypes
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

        private RangeObservableCollection<RoomType> _roomTypes;

        public RangeObservableCollection<RoomType> RoomTypes
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

        private RangeObservableCollection<Agency> _agencies;

        public RangeObservableCollection<Agency> Agencies
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

        private RangeObservableCollection<Operator> _operators;

        public RangeObservableCollection<Operator> Operators
        {
            get
            {
                return _operators;
            }

            set
            {
                _operators = value;
                OnPropertyChanged(() => Operators, false);
            }
        }

        private Reservation _reservation;

        public Reservation Reservation
        {
            get { return _reservation; }
            set
            {
                _reservation = value;
                OnPropertyChanged(() => Reservation, false);
            }
        }

        public bool ViewMode { get; set; }

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

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
