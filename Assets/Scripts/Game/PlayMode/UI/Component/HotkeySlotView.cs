using Core.Resources.Binding.Attributes;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PlayMode.UI.Component
{
    [NeedBinding("UI/Dialogs/PlayMode/HotkeySlot")]
    public class HotkeySlotView : MonoBehaviour
    {
        [ComponentBinding("ItemSlot")]
        private Image _itemSlotImage = null!;
        [ComponentBinding("Highlight")]
        private Image _hotkeyHighlight = null!;
        [ComponentBinding("HotkeySlot")]
        private TextMeshProUGUI _hotkeySlotText = null!;

        public int HotkeyNumber { get; private set; }
        
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
            Highlighted = false;
            HotkeyNumber = hotkeyNumber;
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

        public bool Highlighted
        {
            set => _hotkeyHighlight.SetActive(value);   
        }
    }
}