namespace Game.Inventory.Model
{
    public class InventoryItem
    {
        public string Id { get; }
        public string Name { get; }
        public ItemType ItemType { get; }

        public int HotkeyNumber { get; private set; }

        public InventoryItem(string id, string name, ItemType itemType, int hotkeyNumber = 0)
        {
            Id = id;
            Name = name;
            ItemType = itemType;
            HotkeyNumber = hotkeyNumber;
        }

        public bool TryRebindHotkey(int newHotkey)
        {
            if (HotkeyNumber == newHotkey) {
                HotkeyNumber = 0;
                return false;
            }
            
            HotkeyNumber = newHotkey;
            return true;
        }
    }
}