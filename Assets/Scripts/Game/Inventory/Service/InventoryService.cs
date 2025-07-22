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
    [Core.Attributes.UsedImplicitly]
    public class InventoryService : IInitializable
    {
        private readonly IPublisher<string, InventoryChangedEvent> _publisher;
        private readonly InventoryRepo _inventoryRepo;

        public InventoryService(IPublisher<string, InventoryChangedEvent> publisher, InventoryRepo inventoryRepo)
        {
            _publisher = publisher;
            _inventoryRepo = inventoryRepo;
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

        public bool TryAddToolToInventory(string toolId)
        {
            InventoryItem inventoryItem = new(toolId, toolId, ItemType.TOOL, TryAutoHotkeyItem());
            return TryAddToInventory(inventoryItem);
        }

        private int? TryAutoHotkeyItem()
        {
            return null;
        }
        
        public bool TryAddToInventory(InventoryItem inventoryItem)
        {
            InventoryModel inventoryModel = Inventory;
            if (!inventoryModel.HasFreeSpace) {
                Debug.LogWarning("Max slots reached");
                return false;
            }
            
            if (!inventoryModel.AddItem(inventoryItem)) {
                Debug.LogWarning($"Failed to add item to inventory: {inventoryItem.Name}");
                return false;
            }
            
            _publisher.Publish(InventoryChangedEvent.ADDED, new(inventoryItem));
            _inventoryRepo.Save(inventoryModel);
            Debug.Log($"Added item to inventory: {inventoryItem.Name}");
            return true;
        }

        // todo neiran добавить логирование при провале + constService
        
        public void RemoveFromInventory(InventoryItem inventoryItem)
        {
            InventoryModel inventoryModel = Inventory;
            if (!inventoryModel.RemoveItem(inventoryItem)) {
                Debug.LogWarning($"Failed to remove item from inventory: {inventoryItem.Name}");
                return;
            }
            
            _publisher.Publish(InventoryChangedEvent.REMOVED, new(inventoryItem));
            _inventoryRepo.Save(inventoryModel);
            Debug.Log($"Removed item from inventory: {inventoryItem.Name}");
        }

        // public bool HasFreeHotkeys()
        // {
        //     InventoryModel inventoryModel = _inventoryRepo.Require();
        //     int hotkeysCounter = 0;
        //     foreach (InventorySlot inventorySlot in inventoryModel.InventorySlots) {
        //         if (inventorySlot.InventoryItem == null) {
        //             continue;
        //         }
        //         
        //         if (inventorySlot.InventoryItem.IsHotkey) {
        //             hotkeysCounter++;
        //         }
        //     }
        //     
        //     return hotkeysCounter < Constants.Constants.HOT_KEY_SLOTS;
        // }
        //
        // public int GetFirstPossibleHotkey()
        // {
        //     InventoryModel inventoryModel = _inventoryRepo.Require();
        //     List<int> allHotkeys = new() {
        //             1, 2, 3
        //     };
        //     
        //     foreach (InventorySlot slot in inventoryModel.InventorySlots) {
        //         if (slot.InventoryItem == null) {
        //             continue;
        //         }
        //     }
        // }
        public InventoryModel Inventory => _inventoryRepo.Require();
    }
}