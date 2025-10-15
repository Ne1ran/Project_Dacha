using System.Collections.Generic;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;

namespace Game.Inventory.ViewModel
{
    public class InventoryViewModel
    {
        private readonly InventoryService _inventoryService;
        private readonly ItemsDescriptor _itemsDescriptor;

        public InventoryViewModel(InventoryService inventoryService, ItemsDescriptor itemsDescriptor)
        {
            _inventoryService = inventoryService;
            _itemsDescriptor = itemsDescriptor;
        }

        public List<InventorySlotViewModel> GetCurrentSlotsViewModels()
        {
            List<InventorySlotViewModel> list = new();
            InventoryModel inventory = _inventoryService.Inventory;
            foreach (InventorySlot inventorySlot in inventory.InventorySlots) {
                InventoryItem? inventoryItem = inventorySlot.InventoryItem;
                if (inventoryItem == null) {
                    list.Add(new());
                    continue;
                }

                ItemDescriptorModel toolDescriptor = _itemsDescriptor.Require(inventoryItem.Id);
                list.Add(new(inventoryItem.Id, ItemType.TOOL, toolDescriptor.Icon?.AssetGUID, inventoryItem.HotkeyNumber));
            }

            return list;
        }
    }
}