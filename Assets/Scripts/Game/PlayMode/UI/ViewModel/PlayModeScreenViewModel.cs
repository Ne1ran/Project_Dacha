using System;
using System.Collections.Generic;
using Core.Descriptors.Service;
using Core.Reactive;
using Game.Equipment.Service;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;
using Game.PlayMode.UI.Model;
using Game.Utils;
using MessagePipe;
using UnityEngine;

namespace Game.PlayMode.UI.ViewModel
{
    public class PlayModeScreenViewModel : IDisposable
    {
        private readonly InventoryService _inventoryService;
        private readonly IDescriptorService _descriptorService;
        private readonly EquipmentService _equipmentService;

        private IDisposable? _disposable;

        public ReactiveCollection<HotkeySlotViewModel> Hotkeys { get; } = new(Constants.Constants.HOT_KEY_SLOTS);

        public ReactiveProperty<int> HighlightedHotkey { get; } = new(-1);

        public PlayModeScreenViewModel(InventoryService inventoryService,
                                       IDescriptorService descriptorService,
                                       EquipmentService equipmentService,
                                       ISubscriber<string, InventoryChangedEvent> inventoryChangedSubscriber,
                                       ISubscriber<string, HotkeyChangedEvent> hotkeyChangedSubscriber)
        {
            _inventoryService = inventoryService;
            _descriptorService = descriptorService;
            _equipmentService = equipmentService;

            DisposableBagBuilder bag = DisposableBag.CreateBuilder();

            bag.Add(inventoryChangedSubscriber.Subscribe(InventoryChangedEvent.REMOVED, OnItemRemoved));
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
            for (int i = 1; i < Constants.Constants.HOT_KEY_SLOTS + 1; i++) {
                bool read = false;
                foreach (InventoryItem hotkeyItem in hotkeyItems) {
                    if (hotkeyItem.HotkeyNumber != i) {
                        continue;
                    }

                    list.Add(CreateHotkeySlotViewModel(hotkeyItem, i));

                    read = true;
                    break;
                }

                if (!read) {
                    list.Add(new(i));
                }
            }

            foreach (InventoryItem hotkeyItem in hotkeyItems) {
                int hotkeyNumber = hotkeyItem.HotkeyNumber;
                list[hotkeyNumber - 1] = CreateHotkeySlotViewModel(hotkeyItem, hotkeyNumber);
            }

            return list;
        }

        public void SelectItemFromHotkey(int hotkeyNumber)
        {
            int hotkeyIndex = GetHotkeyIndex(hotkeyNumber);
            HotkeySlotViewModel item = Hotkeys.Collection[hotkeyIndex];
            if (string.IsNullOrEmpty(item.ItemId)) {
                _equipmentService.Unequip();
                HighlightedHotkey.Value = -1;
                return;
            }

            if (_equipmentService.TryEquipItem(item.ItemId)) {
                HighlightedHotkey.Value = Hotkeys.Collection.IndexOf(item);
            } else {
                HighlightedHotkey.Value = -1;
            }
        }

        private void OnHotkeyBinded(HotkeyChangedEvent evt)
        {
            int newHotkeyIndex = GetHotkeyIndex(evt.NewHotkey);
            Hotkeys[newHotkeyIndex] = CreateHotkeySlotViewModel(evt.Item, evt.NewHotkey);
            RemoveOldHotkey(evt.OldHotkey);
        }

        private void OnHotkeyRemoved(HotkeyChangedEvent evt)
        {
            RemoveOldHotkey(evt.OldHotkey);
        }

        private void OnItemRemoved(InventoryChangedEvent evt)
        {
            int hotkeyNumber = evt.Item.HotkeyNumber;
            RemoveOldHotkey(hotkeyNumber);
        }

        private void RemoveOldHotkey(int oldHotkey)
        {
            if (oldHotkey == 0) {
                return;
            }
            
            int oldHotkeyIndex = GetHotkeyIndex(oldHotkey);
            Hotkeys[oldHotkeyIndex] = new(oldHotkey);
            if (HighlightedHotkey.Value == oldHotkeyIndex) {
                HighlightedHotkey.Value = -1;
                // Remove equip from service?
            }
        }

        private int GetHotkeyIndex(int hotkeyNumber)
        {
            for (int i = 0; i < Hotkeys.Collection.Count; i++) {
                HotkeySlotViewModel slotViewModel = Hotkeys.Collection[i];
                if (slotViewModel.HotkeyNumber == hotkeyNumber) {
                    return i;
                }
            }

            Debug.LogWarning($"Hotkey number not found={hotkeyNumber}");
            return 0;
        }

        private HotkeySlotViewModel CreateHotkeySlotViewModel(InventoryItem itemModel, int hotkeyNumber)
        {
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel itemDescriptor = itemsDescriptor.ItemDescriptors.Find(item => itemModel.Id == item.ItemId);
            return new(itemModel.Id, itemModel.ItemType, itemDescriptor.Icon, hotkeyNumber);
        }
    }
}