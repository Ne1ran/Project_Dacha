using System.Collections.Generic;
using Core.Descriptors.Service;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Equipment.Service;
using Game.Inventory.Event;
using Game.Inventory.Service;
using Game.PlayMode.UI.Component;
using Game.PlayMode.UI.Model;
using Game.PlayMode.UI.ViewModel;
using Game.Utils;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.PlayMode.UI.Screen
{
    [NeedBinding("PlayModeScreen")]
    public class PlayModeScreen : MonoBehaviour
    {
        [ComponentBinding("Crosshair")]
        private CrosshairController _crosshairController = null!;
        [ComponentBinding("HotkeyPanel")]
        private HorizontalLayoutGroup _hotkeyPanel = null!;

        [Inject]
        private IResourceService _resourceService = null!;
        [Inject]
        private IDescriptorService _descriptorService = null!;
        [Inject]
        private InventoryService _inventoryService = null!;
        [Inject]
        private InventoryMediator _inventoryMediator = null!;
        [Inject]
        private EquipmentService _equipmentService = null!;
        [Inject]
        private ISubscriber<string, HotkeyChangedEvent> _hotkeyChangedSubscriber = null!;
        [Inject]
        private ISubscriber<string, InventoryChangedEvent> _inventoryChangedSubscriber = null!;

        // todo neiran make it into hotkey panel, take all methods from here and plug it into new panel
        private PlayModeScreenViewModel? _viewModel;

        private List<HotkeySlotView> _hotkeySlotViews = new(Constants.Constants.HotKeySlots);

        public async UniTask InitializeAsync()
        {
            FadeCrosshair(true).Forget();
            _viewModel = new(_inventoryService, _descriptorService, _equipmentService, _inventoryChangedSubscriber, _hotkeyChangedSubscriber);
            await CreateBaseHotkeysAsync();

            _viewModel.Hotkeys.ItemAdded += OnHotkeyAdded;
            _viewModel.Hotkeys.ItemReplaced += OnHotkeyReplaced;
            _viewModel.Hotkeys.ItemRemoved += OnHotkeyRemoved;
            _viewModel.HighlightedHotkey.Changed += OnHotkeyHighlighted;
            _viewModel.Initialize();
        }

        private void OnDestroy()
        {
            if (_viewModel != null) {
                _viewModel.Hotkeys.ItemAdded -= OnHotkeyAdded;
                _viewModel.Hotkeys.ItemReplaced -= OnHotkeyReplaced;
                _viewModel.Hotkeys.ItemRemoved -= OnHotkeyRemoved;
                _viewModel.HighlightedHotkey.Changed -= OnHotkeyHighlighted;
            }

            _viewModel?.Dispose();
            _viewModel = null;
        }

        private void Update()
        {
            if (_inventoryMediator.InventoryOpened) {
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

            if (result is <= 0 or > Constants.Constants.HotKeySlots) {
                return;
            }

            _viewModel!.SelectItemFromHotkey(result);
        }

        private async UniTask CreateBaseHotkeysAsync()
        {
            Transform hotkeyPanel = _hotkeyPanel.transform;
            for (int i = 0; i < hotkeyPanel.childCount; i++) {
                hotkeyPanel.GetChild(i).DestroyObject();
            }

            _hotkeySlotViews.Clear();

            List<HotkeySlotViewModel> hotkeyViewModel = _viewModel!.GetBaseHotkeysViewModels();
            Transform panelTransform = _hotkeyPanel.transform;
            _hotkeySlotViews = await _resourceService.InstantiateAsync<HotkeySlotView>(hotkeyViewModel.Count);
            for (int i = 0; i < hotkeyViewModel.Count; i++) {
                HotkeySlotViewModel slotViewModel = hotkeyViewModel[i];
                HotkeySlotView view = _hotkeySlotViews[i];
                view.Initialize(slotViewModel.HotkeyNumber);
                view.transform.SetParent(panelTransform, false);
            }
        }

        private void OnHotkeyAdded(HotkeySlotViewModel newItem, int index)
        {
            _hotkeySlotViews[index].SetImageAsync(newItem.ImagePath).Forget();
        }

        private void OnHotkeyReplaced(HotkeySlotViewModel oldItem, HotkeySlotViewModel newItem, int index)
        {
            _hotkeySlotViews[index].SetImageAsync(newItem.ImagePath).Forget();
        }

        private void OnHotkeyRemoved(HotkeySlotViewModel oldItem, int index)
        {
            _hotkeySlotViews[index].SetImageAsync(null).Forget();
        }

        private void OnHotkeyHighlighted(int oldIndex, int newIndex)
        {
            for (int i = 0; i < _hotkeySlotViews.Count; i++) {
                HotkeySlotView hotkeySlot = _hotkeySlotViews[i];
                hotkeySlot.Highlighted = i == newIndex;
            }
        }

        public UniTask ShowCrosshair(bool instantly = false)
        {
            return _crosshairController.Show(true);
        }

        public UniTask FadeCrosshair(bool instantly = false)
        {
            return _crosshairController.Fade(true);
        }

        public void SetColor(Color color)
        {
            _crosshairController.ChangeColor(color);
        }
    }
}