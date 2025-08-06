using Game.Inventory.Model;

namespace Game.Inventory.Event
{
    public class HotkeyChangedEvent
    {
        public const string BINDED = "Binded";
        public const string REMOVED = "Removed";

        public InventoryItem Item { get; }
        public int OldHotkey { get; }
        public int NewHotkey { get; }

        public HotkeyChangedEvent(InventoryItem item, int oldHotkey, int newHotkey)
        {
            Item = item;
            OldHotkey = oldHotkey;
            NewHotkey = newHotkey;
        }
    }
}