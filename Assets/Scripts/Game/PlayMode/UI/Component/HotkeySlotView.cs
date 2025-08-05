using Core.Resources.Binding.Attributes;
using Game.Inventory.Model;
using Game.PlayMode.UI.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PlayMode.UI.Component
{
    [PrefabPath("UI/Dialogs/PlayMode/HotkeySlot")]
    public class HotkeySlotView : MonoBehaviour
    {
        [ComponentBinding("ItemSlot")]
        private Image _itemSlotImage;
        [ComponentBinding("HotkeySlot")]
        private TextMeshProUGUI _hotkeySlotText;

        private void Awake()
        {
            HotkeySlotActive = false;
        }

        public void Initialize(HotkeySlotViewModel viewModel)
        {
            Image = viewModel.Image;
            Visible = true;
            HotkeySlotActive = true;
        }

        private string HotkeyText
        {
            set => _hotkeySlotText.text = value;
        }

        private bool HotkeySlotActive
        {
            set => _hotkeySlotText.gameObject.SetActive(value);
        }

        private Sprite Image
        {
            set => _itemSlotImage.sprite = value;
        }

        private bool Visible
        {
            set => _itemSlotImage.color = value ? Color.white : Color.clear;
        }
    }
}