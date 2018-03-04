﻿using Core.Common.Contracts;
using Core.Common.UI.Core;
using System;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System.Windows.Controls;

namespace EchoDesertTrips.Desktop.ViewModels
{
    //[Export]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginControlViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        //[ImportingConstructor]
        public LoginControlViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            LoginCommand = new DelegateCommand<Operator>(OnLoginCommand);
            PasswordChangedCommand = new DelegateCommand<PasswordBox>(OnPasswordChangedCommand);
            CurrentOperator = new Operator();
            ResetCommand = new DelegateCommand<object>(OnResetCommand);
            AuthenticationFailed = false;
            CommunicationFailed = false;
        }

        public event EventHandler<AuthenticationEventArgs> Authenticated;
        public DelegateCommand<Operator> LoginCommand { get; set; }
        public DelegateCommand<PasswordBox> PasswordChangedCommand { get; set; }

        public DelegateCommand<object> ResetCommand { get; set; }

        private void OnResetCommand(object obj)
        {
            AuthenticationFailed = false;
            CommunicationFailed = false;
        }


        private void OnLoginCommand(Operator op)
        {
            try
            {
                WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                {
                    var oper = operatorClient.GetOperator(op.OperatorName, op.Password);
                    if (oper != null)
                    {
                        Authenticated?.Invoke(this, new AuthenticationEventArgs(oper));
                    }
                    else
                        AuthenticationFailed = true;
                });
            }
            catch (Exception ex)
            {
                log.Error("Exception Get Operator: " + ex.Message);
                CommunicationFailed = true;
            }
        }

        protected override void OnViewLoaded()
        {
        }



        private bool _authenticationFailed;
        public bool AuthenticationFailed
        {
            get
            {
                return _authenticationFailed;
            }
            set
            {
                if (_authenticationFailed != value)
                {
                    _authenticationFailed = value;
                     OnPropertyChanged(() => AuthenticationFailed);
                }
            }
        }

        private bool _communicationFailed;
        public bool CommunicationFailed
        {
            get
            {
                return _communicationFailed;
            }
            set
            {
                if (_communicationFailed != value)
                {
                    _communicationFailed = value;
                    OnPropertyChanged(() => CommunicationFailed);
                }
            }
        }

        private Operator _currentOperator;

        public Operator CurrentOperator
        {
            get
            {
                return _currentOperator;
            }
            set
            {
                if (_currentOperator != value)
                {
                    _currentOperator = value;
                    OnPropertyChanged(() => CurrentOperator);
                }
            }
        }

        private void OnPasswordChangedCommand(PasswordBox obj)
        {
            if (obj != null)
                _currentOperator.Password = obj.Password;
        }
    }
}
