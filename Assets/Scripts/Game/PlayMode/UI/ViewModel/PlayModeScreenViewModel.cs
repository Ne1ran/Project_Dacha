using System;
using System.Collections.Generic;
using Core.Descriptors.Service;
using Core.Reactive;
using Game.Descriptors;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.PlayMode.UI.Model;
using MessagePipe;
using UnityEngine;

namespace Game.PlayMode.UI.ViewModel
{
    public class PlayModeScreenViewModel : IDisposable
    {
        private readonly InventoryService _inventoryService;
        private readonly IDescriptorService _descriptorService;

        private IDisposable? _disposable;
        
        public ReactiveCollection<HotkeySlotViewModel> Hotkeys { get; } = new(Constants.Constants.HOT_KEY_SLOTS);

        public PlayModeScreenViewModel(InventoryService inventoryService,
                                       IDescriptorService descriptorService,
                                       ISubscriber<string, HotkeyChangedEvent> hotkeyChangedSubscriber)
        {
            _inventoryService = inventoryService;
            _descriptorService = descriptorService;

            DisposableBagBuilder bag = DisposableBag.CreateBuilder();
            
            bag.Add(hotkeyChangedSubscriber.Subscribe(HotkeyChangedEvent.BINDED, OnHotkeyBinded));
            bag.Add(hotkeyChangedSubscriber.Subscribe(HotkeyChangedEvent.REMOVED, OnHotkeyRemoved));
            
            _disposable = bag.Build();
        }

        public void Initialize()
        {
            List<HotkeySlotViewModel> hotkeyItems = GetCurrentHotkeyItems();

            foreach (HotkeySlotViewModel hotkeySlotViewModel in hotkeyItems) {
                Hotkeys.Add(hotkeySlotViewModel);
            }
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public List<HotkeySlotViewModel> GetBaseHotkeysViewModels()
        {
            List<HotkeySlotViewModel> list = new();
            for (int i = 1; i < Constants.Constants.HOT_KEY_SLOTS + 1; i++) {
                list.Add(new(i));
            }

            return list;
        }

        public List<HotkeySlotViewModel> GetCurrentHotkeyItems()
        {
            List<HotkeySlotViewModel> list = new();

            List<InventoryItem> hotkeyItems = _inventoryService.GetHotkeyItems();
            int hotkeyItemsCount = hotkeyItems.Count;
            for (int i = 1; i < Constants.Constants.HOT_KEY_SLOTS + 1; i++) {
                if (hotkeyItemsCount < i) {
                    list.Add(new(i));
                    continue;
                }

                InventoryItem inventoryItem = hotkeyItems[i - 1];
                if (inventoryItem == null) {
                    Debug.LogWarning("InventoryItem somehow is null but it has hotkey number");
                    continue;
                }

                switch (inventoryItem.ItemType) {
                    case ItemType.TOOL:
                        list.Add(CreateToolSlotViewModel(inventoryItem.Id, i));
                        break;
                    default:
                        // todo neiran add for future item types
                        list.Add(new(i));
                        break;
                }
            }

            return list;
        }

        private void OnHotkeyBinded(HotkeyChangedEvent evt)
        {
            int hotkeyNumber = evt.NewHotkey - 1;
            Hotkeys[hotkeyNumber] = CreateToolSlotViewModel(evt.Item.Id, hotkeyNumber);
        }

        private void OnHotkeyRemoved(HotkeyChangedEvent evt)
        {
            int hotkeyNumber = evt.OldHotkey - 1;
            Hotkeys[hotkeyNumber] = new(hotkeyNumber);
        }

        private HotkeySlotViewModel CreateToolSlotViewModel(string toolId, int hotkeyNumber)
        {
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            ToolsDescriptorModel toolDescriptor = toolsDescriptor.ToolsDescriptors.Find(tool => tool.ToolId == toolId);
            return new(toolId, ItemType.TOOL, toolDescriptor.ToolIcon, hotkeyNumber);
        }
    }
}