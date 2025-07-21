namespace Game.Inventory.Model
{
    public class InventoryItem
    {
        public string Id { get; }
        public string Name { get; }
        public ItemType ItemType { get; }
        public bool IsHotkey { get;}
        
        public int ItemSlot { get; set; }

        public InventoryItem(string id, string name, ItemType itemType, bool isHotkey)
        {
            Id = id;
            Name = name;
            ItemType = itemType;
            IsHotkey = isHotkey;
        }
    }
}