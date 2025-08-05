namespace Game.Inventory.Model
{
    public class InventoryItem
    {
        public string Id { get; }
        public string Name { get; }
        public ItemType ItemType { get; }
        
        public int HotkeyNumber { get; }

        public InventoryItem(string id, string name, ItemType itemType, int hotkeyNumber = 0)
        {
            Id = id;
            Name = name;
            ItemType = itemType;
            HotkeyNumber = hotkeyNumber;
        }
    }
}