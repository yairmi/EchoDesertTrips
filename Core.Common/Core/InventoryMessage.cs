using static Core.Common.Core.Const;

namespace Core.Common.Core
{
    public class InventoryMessage
    {
        public InventoryMessage() { }
        public InventoryMessage(eInventoryTypes inventoryType, int entityId)
        {
            InventoryType = inventoryType;
            EntityId = entityId;
        }
        public eInventoryTypes InventoryType { get; set; }
        public int EntityId { get; set; }
    }
}
