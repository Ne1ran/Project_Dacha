using UnityEngine.AddressableAssets;

namespace Game.Inventory.Model
{
    public class InventorySlotViewModel
    {
        public string? ItemId { get; }
        public ItemType ItemType { get; }
        public string? Image { get; }
        public int HotkeyNumber { get; }

        public InventorySlotViewModel(string itemId, ItemType itemType, string? image, int hotkeyNumber)
        {
            ItemId = itemId;
            ItemType = itemType;
            Image = image;
            HotkeyNumber = hotkeyNumber;
        }

        public InventorySlotViewModel()
        {
        }
    }
}