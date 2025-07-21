using Core.Attributes;
using Game.Inventory.Event;
using Game.Inventory.Model;
using Game.Inventory.Repo;
using MessagePipe;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace Game.Inventory.Service
{
    [UsedImplicitly]
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
            
            _inventoryRepo.Save(new());
        }
        
        public void AddToInventory(InventoryItem inventoryItem)
        {
            InventoryModel inventoryModel = _inventoryRepo.Require();
            if (!inventoryModel.AddItem(inventoryItem)) {
                Debug.LogWarning($"Failed to add item to inventory: {inventoryItem.Name}");
                return;
            }
            
            _publisher.Publish(InventoryChangedEvent.ADDED, new(inventoryItem));
            _inventoryRepo.Save(inventoryModel);
            Debug.Log($"Added item to inventory: {inventoryItem.Name}");
        }

        // todo neiran добавить логирование при провале + constService
        
        public void RemoveFromInventory(InventoryItem inventoryItem)
        {
            InventoryModel inventoryModel = _inventoryRepo.Require();
            if (!inventoryModel.RemoveItem(inventoryItem)) {
                Debug.LogWarning($"Failed to remove item from inventory: {inventoryItem.Name}");
                return;
            }
            _publisher.Publish(InventoryChangedEvent.REMOVED, new(inventoryItem));
            _inventoryRepo.Save(inventoryModel);
            Debug.Log($"Removed item from inventory: {inventoryItem.Name}");
        }
    }
}