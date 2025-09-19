using System;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Inventory.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Game.Inventory.UI
{
    [NeedBinding("InventorySlot")]
    public class InventorySlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [ComponentBinding("ItemSlot")]
        private Image _itemSlotImage = null!;
        [ComponentBinding("HotkeySlot")]
        private TextMeshProUGUI _hotkeySlotText = null!;

        [Inject]
        private readonly IResourceService _resourceService = null!;
        
        private int _slotIndex;
        private int _currentHotkey;
        private bool _waitingForInput;
        private bool _hasItem;

        public event Action<int>? Dropped;
        public event Action<int, int>? Binded;
        
        private void Start()
        {
            HotkeySlotActive = false;
        }

        public async UniTask InitializeAsync(InventorySlotViewModel viewModel, int slotIndex)
        {
            _slotIndex = slotIndex;
            _hasItem = !string.IsNullOrEmpty(viewModel.ItemId);

            if (!string.IsNullOrEmpty(viewModel.Image)) {
                Image = await _resourceService.LoadAssetAsync<Sprite>(viewModel.Image);
                ImageVisible = true;
            } else {
                ImageVisible = false;
            }
            
            int hotkeyNumber = viewModel.HotkeyNumber;
            if (hotkeyNumber == 0) {
                return;
            }

            HotkeySlotActive = true;
            _currentHotkey = hotkeyNumber;
            HotkeyText = hotkeyNumber.ToString();
        }

        public void Clear()
        {
            _currentHotkey = 0;
            _hasItem = false;
            Image = null;
            ImageVisible = false;
            HotkeySlotActive = false;
        }
        
        public void UpdateHotkey(int hotkeyNumber)
        {
            _currentHotkey = hotkeyNumber;
            if (hotkeyNumber == 0) {
                HotkeySlotActive = false;
                return;
            }
            
            HotkeySlotActive = true;
            HotkeyText = hotkeyNumber.ToString();
        }

        private void Update()
        {
            if (!_hasItem) {
                return;
            }
            
            if (!_waitingForInput) {
                return;
            }
            
            string input = Input.inputString;
            if (string.IsNullOrEmpty(input)) {
                return;
            }

            if (input.Length != 1) {
                return;
            }

            char currentInputChar = input[0];
            if (!int.TryParse(currentInputChar.ToString(), out int result)) {
                return;
            }

            if (result is <= 0 or > Constants.Constants.HOT_KEY_SLOTS) {
                return;
            }
            
            Binded?.Invoke(_slotIndex, result);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _waitingForInput = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _waitingForInput = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_hasItem) {
                return;
            }
            
            if (eventData.button == PointerEventData.InputButton.Right) {
                Dropped?.Invoke(_slotIndex);
            }
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
        
        public int CurrentHotkey => _currentHotkey;
    }
}