using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Core;
using FluentValidation.Results;
using EchoDesertTrips.Client.Entities;
using static Core.Common.Core.Const;
using EchoDesertTrips.Common;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Core.Common.UI.Core
{
    public class ViewModelBase : ObjectBase
    {
        public ViewModelBase()
        {
            ToggleErrorsCommand = new DelegateCommand<object>(OnToggleErrorsCommandExecute, OnToggleErrorsCommandCanExecute);
            if (_eventAggregator == null)
            {
                _eventAggregator = new EventAggregator();
            }
        }

        static protected IEventAggregator _eventAggregator;

        public InventoriesSingle Inventories
        {
            get
            {
                return InventoriesSingle.Instance;
            }
        }

        public OperatorSingle CurrentOperator
        {
            get
            {
                return OperatorSingle.Instance;
            }
        }

        public ClientSingle Client = ClientSingle.Instance;

        bool _ErrorsVisible = false;

        public object ViewLoaded
        {
            get
            {
                OnViewLoaded();
                return null;
            }
        }

        public virtual void OnViewUnloaded() { }

        protected virtual void OnViewLoaded() { }

        protected void WithClient<T>(T proxy, Action<T> codeToExecute)
        {
            try
            {
                codeToExecute.Invoke(proxy);
            }
            catch (Exception ex)
            {
                log.Error(string.Empty, ex);
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

        protected string SerializeInventoryMessage(eInventoryTypes inventoryType, eOperation operation, int EntityId)
        {
            InventoryMessage invMsg = new InventoryMessage(inventoryType, operation, EntityId);
            var ser = new Serializer();
            return ser.Serialize<InventoryMessage>(invMsg);
        }

        protected string SerializeReservationMessage(int EntityId, eOperation operation)
        {
            var reservationsMessage = new ReservationsMessage()
            {
                ReservationsIds = new List<ReservationMessage>()
            };
            reservationsMessage.ReservationsIds.Add(new ReservationMessage(EntityId, operation));
            Serializer ser = new Serializer();
            return ser.Serialize(reservationsMessage);
        }

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
