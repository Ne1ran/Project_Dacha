using Game.Inventory.Model;

namespace Game.Inventory.Event
{
    public class InventoryChangedEvent
    {
        public const string ADDED = "ItemAdded";
        public const string REMOVED = "ItemRemoved";

        public InventoryItem Item { get; }

        public InventoryChangedEvent(InventoryItem item)
        {
            Item = item;
        }
    }
}