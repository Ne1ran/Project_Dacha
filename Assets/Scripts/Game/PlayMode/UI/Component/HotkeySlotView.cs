using Core.Resources.Binding.Attributes;
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
        private Image _itemSlotImage = null!;
        [ComponentBinding("HotkeySlot")]
        private TextMeshProUGUI _hotkeySlotText = null!;

        private void Awake()
        {
            HotkeySlotActive = false;
        }

        public void Initialize(int hotkeyNumber)
        {
            Image = null;
            ImageVisible = false;
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

        public Sprite? Image
        {
            set
            {
                _itemSlotImage.sprite = value;
                ImageVisible = value != null;
            }
        }

        private bool ImageVisible
        {
            set => _itemSlotImage.color = value ? Color.white : Color.clear;
        }
    }
}