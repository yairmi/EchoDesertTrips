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
        
        //public static int MSG_TYPE_RESERVATION => 1;
        //public static int MSG_TYPE_INVENTORY => 2;

        //public static eInventoryTypes INVENTORY_ROOM_TYPE => 1;
        //public static int INVENTORY_HOTEL => 2;
        //public static int INVENTORY_OPTIONAL => 3;
        //public static int INVENTORY_OPERATOR => 4;
        //public static int INVENTORY_AGENCY => 5;
    }
}
