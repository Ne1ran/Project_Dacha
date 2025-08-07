using Game.Items.Model;

namespace Game.Equipment.Event
{
    public class EquipmentChangedEvent
    {
        public const string EQUIPMENT_CHANGED = "EquipmentChanged";
        public const string EQUIPMENT_DROPPED = "EquipmentDropped";
        
        public ItemModel? OldItem { get; }
        public ItemModel? NewItem { get; }

        public EquipmentChangedEvent(ItemModel? oldItem = null, ItemModel? newItem = null)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}