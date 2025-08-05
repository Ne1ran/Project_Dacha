using Game.Inventory.Model;
using UnityEngine;

namespace Game.PlayMode.UI.Model
{
    public class HotkeySlotViewModel
    {
        public string? ItemId { get; }
        public ItemType ItemType { get; }
        public Sprite? Image { get; }
        public int HotkeyNumber { get; }

        public HotkeySlotViewModel(string itemId, ItemType itemType, Sprite image, int hotkeyNumber)
        {
            ItemId = itemId;
            ItemType = itemType;
            Image = image;
            HotkeyNumber = hotkeyNumber;
        }

        public HotkeySlotViewModel(int hotkeyNumber)
        {
            HotkeyNumber = hotkeyNumber;
        }
    }
}