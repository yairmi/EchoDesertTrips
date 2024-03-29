﻿using EchoDesertTrips.Client.Entities;
using System;

namespace Core.Common.UI.CustomEventArgs
{
    public class OperatorEventArgs : EventArgs
    {
        public OperatorEventArgs(Operator oper, bool isNew)
        {
            Operator = oper;
            bIsNew = isNew;
        }
        public Operator Operator { get; private set; }
        public bool bIsNew { get; private set; }
    }
}
