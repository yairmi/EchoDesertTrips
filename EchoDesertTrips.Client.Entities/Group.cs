using Core.Common.Core;
using System;

namespace EchoDesertTrips.Client.Entities
{
    public class Group : ObjectBase
    {
        private int _groupId;
        public int GroupId
        {
            get
            {
                return _groupId;
            }
            set
            {
                _groupId = value;
                OnPropertyChanged(() => GroupId);
            }
        }

        private string _externalId;

        public string ExternalId
        {
            get
            {
                return _externalId;
            }
            set
            {
                _externalId = value;
                OnPropertyChanged(() => ExternalId);
            }
        }


    }
}
