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
        private Image _itemSlotImage;
        [ComponentBinding("HotkeySlot")]
        private TextMeshProUGUI _hotkeySlotText;

        private void Awake()
        {
            HotkeySlotActive = false;
        }

        public void Initialize(InventorySlotViewModel viewModel)
        {
            _itemSlotImage.sprite = viewModel.Image;
            int? hotkeyNumber = viewModel.HotkeyNumber;
            if (hotkeyNumber == null) {
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
    }
}