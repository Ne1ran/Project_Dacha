using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.PlayMode.UI.Component
{
    [NeedBinding("HotkeySlot")]
    public class HotkeySlotView : MonoBehaviour
    {
        [ComponentBinding("ItemSlot")]
        private Image _itemSlotImage = null!;
        [ComponentBinding("Highlight")]
        private Image _hotkeyHighlight = null!;
        [ComponentBinding("SlotText")]
        private TextMeshProUGUI _hotkeySlotText = null!;

        [Inject]
        private readonly IResourceService _resourceService = null!;

        public int HotkeyNumber { get; private set; }

        private Sprite? _currentImage;

        public void Initialize(int hotkeyNumber)
        {
            SetImageAsync(null).Forget();
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

        public async UniTaskVoid SetImageAsync(string? newImage)
        {
            if (_currentImage != null) {
                _resourceService.Release(_currentImage);
            }
            
            if (newImage == null) {
                _itemSlotImage.sprite = null;
                ImageVisible = false;
                _currentImage = null;
            } else {
                _currentImage = await _resourceService.LoadAssetAsync<Sprite>(newImage);
                ImageVisible = true;
            }

            Image = _currentImage;
        }

        private Sprite? Image
        {
            set => _itemSlotImage.sprite = value;
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