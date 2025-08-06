using System.Collections.Generic;
using Core.Descriptors.Service;
using Game.Descriptors;
using Game.Inventory.Model;
using Game.Inventory.Service;

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

            InventoryModel inventory = _inventoryService.Inventory;
            foreach (InventorySlot inventorySlot in inventory.InventorySlots) {
                InventoryItem inventoryItem = inventorySlot.InventoryItem;
                if (inventoryItem == null) {
                    list.Add(new());
                    continue;
                }

                switch (inventoryItem.ItemType) {
                    case ItemType.TOOL:
                        list.Add(CreateToolSlotViewModel(inventoryItem.Id, inventoryItem.HotkeyNumber));
                        break;
                    default:
                        // todo neiran add for future item types
                        list.Add(new());
                        break;
                }
            }

            return list;
        }

        private InventorySlotViewModel CreateToolSlotViewModel(string toolId, int hotkeyNumber)
        {
            ToolsDescriptor toolsDescriptor = _descriptorService.Require<ToolsDescriptor>();
            ToolsDescriptorModel toolDescriptor = toolsDescriptor.ToolsDescriptors.Find(tool => tool.ToolId == toolId);
            return new(toolId, ItemType.TOOL, toolDescriptor.ToolIcon, hotkeyNumber);
        }
    }
}