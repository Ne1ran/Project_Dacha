using JetBrains.Annotations;
using UnityEngine;

namespace Game.Inventory.Model
{
    public class InventorySlotViewModel
    {
        [CanBeNull]
        public string ItemId { get; }
        public ItemType ItemType { get; }
        [CanBeNull]
        public Sprite Image { get; }
        public int? HotkeyNumber { get; }

        public InventorySlotViewModel(string itemId, ItemType itemType, Sprite image, int? hotkeyNumber)
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