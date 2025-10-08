using System.Collections.Generic;
using Core.Descriptors.Service;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;

namespace Game.Inventory.ViewModel
{
    public class InventoryViewModel
    {
        private readonly InventoryService _inventoryService;
        private readonly IDescriptorService _descriptorService;

        public InventoryViewModel(InventoryService inventoryService, IDescriptorService descriptorService)
        {
            _inventoryService = inventoryService;
            _descriptorService = descriptorService;
        }

        public List<InventorySlotViewModel> GetCurrentSlotsViewModels()
        {
            List<InventorySlotViewModel> list = new();
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            InventoryModel inventory = _inventoryService.Inventory;
            foreach (InventorySlot inventorySlot in inventory.InventorySlots) {
                InventoryItem? inventoryItem = inventorySlot.InventoryItem;
                if (inventoryItem == null) {
                    list.Add(new());
                    continue;
                }
                
                ItemDescriptorModel toolDescriptor = itemsDescriptor.Require(inventoryItem.Id);
                list.Add(new(inventoryItem.Id, ItemType.TOOL, toolDescriptor.Icon?.AssetGUID, inventoryItem.HotkeyNumber));
            }

            return list;
        }
    }
}