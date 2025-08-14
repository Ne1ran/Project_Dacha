using System.Collections.Generic;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Inventory.Repo;
using JetBrains.Annotations;
using MessagePipe;
using UnityEngine;
using VContainer.Unity;

namespace Game.Inventory.Service
{
    [UsedImplicitly]
    public class InventoryService : IInitializable
    {
        private readonly IPublisher<string, InventoryChangedEvent> _inventoryChangedPublisher;
        private readonly IPublisher<string, HotkeyChangedEvent> _hotkeyChangedPublisher;
        private readonly InventoryRepo _inventoryRepo;

        public InventoryService(IPublisher<string, InventoryChangedEvent> inventoryChangedPublisher,
                                InventoryRepo inventoryRepo,
                                IPublisher<string, HotkeyChangedEvent> hotkeyChangedPublisher)
        {
            _inventoryChangedPublisher = inventoryChangedPublisher;
            _inventoryRepo = inventoryRepo;
            _hotkeyChangedPublisher = hotkeyChangedPublisher;
        }

        public void Initialize()
        {
            if (_inventoryRepo.Exists()) {
                return;
            }

            List<InventorySlot> inventory = new(Constants.Constants.INVENTORY_SLOTS);
            for (int i = 0; i < Constants.Constants.INVENTORY_SLOTS; i++) {
                inventory.Add(new());
            }

            _inventoryRepo.Save(new(inventory));
        }

        public bool TryAddItemToInventory(string itemId, ItemType itemType)
        {
            InventoryModel inventoryModel = Inventory;
            if (!inventoryModel.HasFreeSpace) {
                Debug.LogWarning("Max slots reached");
                return false;
            }
            int autoHotkeyNumber = TryAutoHotkeyItem(inventoryModel.InventorySlots);
            InventoryItem inventoryItem = new(itemId, itemId, itemType, autoHotkeyNumber);
            bool result = TryAddToInventory(inventoryItem);
            if (result && autoHotkeyNumber != 0) {
                _hotkeyChangedPublisher.Publish(HotkeyChangedEvent.BINDED, new(inventoryItem, 0, autoHotkeyNumber));
            }

            return result;
        }

        public bool TryChangeHotkey(int slotIndex, int newHotkey)
        {
            InventoryModel inventory = Inventory;

            InventorySlot slotForBind = inventory.InventorySlots[slotIndex];
            InventorySlot? currentBindedSlot = inventory.InventorySlots.Find(slot => slot.InventoryItem?.HotkeyNumber == newHotkey);

            InventoryItem? inventoryItem = slotForBind.InventoryItem;
            if (inventoryItem == null) {
                Debug.LogWarning($"Can't change hotkey to slot without any items. SlotIndex={slotIndex}");
                return false;
            }

            int currentHotkey = inventoryItem.HotkeyNumber;
            if (!inventoryItem.TryRebindHotkey(newHotkey)) {
                Debug.Log("Hotkey double binded which means we remove it.");
                _inventoryRepo.Save(inventory);
                _hotkeyChangedPublisher.Publish(HotkeyChangedEvent.REMOVED, new(inventoryItem, currentHotkey, 0));
                return false;
            }

            if (currentBindedSlot != null) {
                InventoryItem oldBindedItem = currentBindedSlot.InventoryItem!;
                int oldHotkey = oldBindedItem.HotkeyNumber;
                oldBindedItem.TryRebindHotkey(0);
                _hotkeyChangedPublisher.Publish(HotkeyChangedEvent.REMOVED, new(inventoryItem, oldHotkey, 0));
            }
            
            _hotkeyChangedPublisher.Publish(HotkeyChangedEvent.BINDED, new(inventoryItem, currentHotkey, newHotkey));
            _inventoryRepo.Save(inventory);
            return true;
        }

        public List<InventoryItem> GetHotkeyItems()
        {
            List<InventoryItem> hotkeyItems = new();

            InventoryModel inventory = Inventory;
            inventory.InventorySlots.ForEach(slot => {
                InventoryItem? inventoryItem = slot.InventoryItem;
                if (inventoryItem != null && inventoryItem.HotkeyNumber != 0) {
                    hotkeyItems.Add(inventoryItem);
                }
            });

            hotkeyItems.Sort((item1, item2) => item1.HotkeyNumber > item2.HotkeyNumber ? 1 : -1);
            return hotkeyItems;
        }

        private int TryAutoHotkeyItem(List<InventorySlot> inventorySlots)
        {
            List<int> occupiedHotkeys = new(Constants.Constants.HOT_KEY_SLOTS);
            foreach (InventorySlot slot in inventorySlots) {
                if (slot.InventoryItem == null) {
                    continue;
                }

                int hotkeyNumber = slot.InventoryItem.HotkeyNumber;
                if (hotkeyNumber != 0) {
                    occupiedHotkeys.Add(hotkeyNumber);
                }
            }

            for (int i = 1; i <= Constants.Constants.HOT_KEY_SLOTS; i++) {
                if (!occupiedHotkeys.Contains(i)) {
                    return i;
                }
            }

            return 0;
        }

        public bool TryAddToInventory(InventoryItem inventoryItem)
        {
            InventoryModel inventoryModel = Inventory;
            if (!inventoryModel.AddItem(inventoryItem)) {
                Debug.LogWarning($"Failed to add item to inventory: {inventoryItem.Name}");
                return false;
            }

            _inventoryChangedPublisher.Publish(InventoryChangedEvent.ADDED, new(inventoryItem));
            _inventoryRepo.Save(inventoryModel);
            Debug.Log($"Added item to inventory: {inventoryItem.Name}");
            return true;
        }

        // todo neiran добавить логирование при провале + constService

        public bool TryRemoveFromSlot(int slotIndex)
        {
            InventoryModel inventory = Inventory;
            InventorySlot? inventorySlot = inventory.InventorySlots[slotIndex];
            if (inventorySlot == null) {
                Debug.LogWarning($"Failed to remove item from inventory. Slot not found slotId={slotIndex}");
                return false;
            }

            InventoryItem? inventoryItem = inventorySlot.InventoryItem;
            if (inventoryItem == null) {
                Debug.Log($"Item was already null when remove from slot={slotIndex}");
                return false;
            }
            
            inventorySlot.InventoryItem = null;
            _inventoryChangedPublisher.Publish(InventoryChangedEvent.REMOVED, new(inventoryItem));
            _inventoryRepo.Save(inventory);
            Debug.Log($"Removed item from inventory: {inventoryItem.Name}");
            return true;
        }

        public void RemoveFromInventory(InventoryItem inventoryItem)
        {
            InventoryModel inventoryModel = Inventory;
            if (!inventoryModel.RemoveItem(inventoryItem)) {
                Debug.LogWarning($"Failed to remove item from inventory: {inventoryItem.Name}");
                return;
            }

            _inventoryChangedPublisher.Publish(InventoryChangedEvent.REMOVED, new(inventoryItem));
            _inventoryRepo.Save(inventoryModel);
            Debug.Log($"Removed item from inventory: {inventoryItem.Name}");
        }

        public InventoryModel Inventory => _inventoryRepo.Require();
    }
}