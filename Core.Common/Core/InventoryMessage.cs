using static Core.Common.Core.Const;

namespace Core.Common.Core
{
    public class InventoryMessage
    {
        public InventoryMessage() { }
        public InventoryMessage(eInventoryTypes inventoryType, eOperation operation, int entityId)
        {
            InventoryType = inventoryType;
            Operation = operation;
            EntityId = entityId;
        }
        public eInventoryTypes InventoryType { get; set; }
        public int EntityId { get; set; }
        public eOperation Operation { get; set; }
    }
}
