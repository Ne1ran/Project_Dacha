using System.Collections.Generic;
using System.Linq;
using Core.Descriptors.Service;
using Core.Resources.Binding.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Inventory.ViewModel;
using MessagePipe;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Inventory.UI
{
    [NeedBinding("InventoryDialog")]
    public class InventoryDialog : MonoBehaviour
    {
        [ComponentBinding("CloseButton")]
        private Button _closeButton = null!;
        [ComponentBinding("Background")]
        private ClickableControl _background = null!;
        [ComponentBinding("MainPanel")]
        private Transform _mainPanel = null!;

        [Inject]
        private InventoryService _inventoryService = null!;
        [Inject]
        private IResourceService _resourceService = null!;
        [Inject]
        private IDescriptorService _descriptorService = null!;
        [Inject]
        private IPublisher<string, InventoryStatusEvent> _inventoryPublisher = null!;

        private List<InventorySlotView> _inventorySlots = new();

        private void Awake()
        {
            _closeButton.onClick.AddListener(OnCloseTriggered);
            _background.OnClick += OnCloseTriggered;
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(OnCloseTriggered);
            _background.OnClick -= OnCloseTriggered;
        }

        public async UniTask InitializeAsync()
        {
            InventoryViewModel inventoryViewModel = new(_inventoryService, _descriptorService);
            List<InventorySlotViewModel> slots = inventoryViewModel.GetCurrentSlotsViewModels();

            List<UniTask<InventorySlotView>> loadTasks = new(slots.Count);
            for (int i = 0; i < slots.Count; i++) {
                loadTasks.Add(_resourceService.InstantiateAsync<InventorySlotView>());
            }

            _inventorySlots = (await UniTask.WhenAll(loadTasks)).ToList();
            for (int i = 0; i < slots.Count; i++) {
                InventorySlotViewModel slotViewModel = slots[i];
                InventorySlotView view = _inventorySlots[i];
                view.Initialize(slotViewModel, i);
                view.Dropped += OnItemDropped;
                view.Binded += OnItemBinded;
                view.transform.SetParent(_mainPanel, false);
            }
        }

        private void OnItemDropped(int slotIndex)
        {
            if (_inventoryService.TryRemoveFromSlot(slotIndex)) {
                _inventorySlots[slotIndex].Clear();
            }
        }

        private void OnItemBinded(int slotIndex, int hotkeyNumber)
        {
            if (_inventoryService.TryChangeHotkey(slotIndex, hotkeyNumber)) {
                InventorySlotView? slotView = _inventorySlots.Find(slot => slot.CurrentHotkey == hotkeyNumber);
                if (slotView != null) {
                    slotView.UpdateHotkey(0);    
                }
                
                _inventorySlots[slotIndex].UpdateHotkey(hotkeyNumber);
            } else {
                _inventorySlots[slotIndex].UpdateHotkey(0);
            }
        }

        private void OnCloseTriggered()
        {
            _inventoryPublisher.Publish(InventoryStatusEvent.INVENTORY_CHANGED, new());
        }
    }
}