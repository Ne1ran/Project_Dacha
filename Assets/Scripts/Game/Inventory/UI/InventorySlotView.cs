using Core.Resources.Binding.Attributes;
using Game.Inventory.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory.UI
{
    [PrefabPath("UI/Dialogs/Inventory/InventorySlot")]
    public class InventorySlotView : MonoBehaviour
    {
        [ComponentBinding("ItemSlot")]
        private Image _itemSlotImage = null!;
        [ComponentBinding("HotkeySlot")]
        private TextMeshProUGUI _hotkeySlotText = null!;

        private void Awake()
        {
            HotkeySlotActive = false;
        }

        public void Initialize(InventorySlotViewModel viewModel)
        {
            Image = viewModel.Image;
            ImageVisible = viewModel.Image != null;
            int hotkeyNumber = viewModel.HotkeyNumber;
            if (hotkeyNumber == 0) {
                return;
            }

            HotkeySlotActive = true;
            HotkeyText = hotkeyNumber.ToString();
        }

        private string HotkeyText
        {
            set => _hotkeySlotText.text = value;
        }

        private bool HotkeySlotActive
        {
            set => _hotkeySlotText.gameObject.SetActive(value);
        }

        private Sprite? Image
        {
            set => _itemSlotImage.sprite = value;
        }

        private bool ImageVisible
        {
            set => _itemSlotImage.color = value ? Color.white : Color.clear;
        }
    }
}