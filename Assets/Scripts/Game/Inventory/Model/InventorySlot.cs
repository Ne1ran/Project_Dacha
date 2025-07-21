using JetBrains.Annotations;

namespace Game.Inventory.Model
{
    public class InventorySlot
    {
        [CanBeNull]
        public InventoryItem InventoryItem { get; set; }
    }
}