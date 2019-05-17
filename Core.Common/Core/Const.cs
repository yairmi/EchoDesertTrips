using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Core
{
    public static class Const
    {
        public enum eMsgTypes { E_RESERVATION, E_INVENTORY }
        public enum eInventoryTypes { E_TOUR_TYPE, E_ROOM_TYPE, E_HOTEL, E_OPTIONAL, E_OPERATOR, E_AGENCY};
        public enum eOperation { E_ADDED, E_UPDATED, E_DELETED};

        public const int MAX_NUMBER_OF_CUSTOMERS = 8192;
    }
}
