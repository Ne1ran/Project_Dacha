namespace Game.Inventory.Model
{
    public class InventoryItem
    {
        public string Id { get; }
        public string Name { get; }
        public ItemType ItemType { get; }
        
        public int? HotkeyNumber { get; set; }

        public InventoryItem(string id, string name, ItemType itemType, int? hotkeyNumber)
        {
            Id = id;
            Name = name;
            ItemType = itemType;
            HotkeyNumber = hotkeyNumber;
        }

        public bool IsHotkey => HotkeyNumber != null;
    }
}