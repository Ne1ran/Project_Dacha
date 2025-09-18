using Game.Inventory.Model;

namespace Game.PlayMode.UI.Model
{
    public class HotkeySlotViewModel
    {
        public string? ItemId { get; }
        public ItemType ItemType { get; }
        public string? ImagePath { get; }
        public int HotkeyNumber { get; }

        public HotkeySlotViewModel(string itemId, ItemType itemType, string? imagePath, int hotkeyNumber)
        {
            ItemId = itemId;
            ItemType = itemType;
            ImagePath = imagePath;
            HotkeyNumber = hotkeyNumber;
        }

        public HotkeySlotViewModel(int hotkeyNumber)
        {
            HotkeyNumber = hotkeyNumber;
        }
    }
}