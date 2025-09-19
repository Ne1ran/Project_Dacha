using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Inventory.Service;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Utils;

namespace Game.Items.Service
{
    [Service]
    public class PickUpItemService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly InventoryService _inventoryService;

        public PickUpItemService(IDescriptorService descriptorService,
                                 InventoryService inventoryService)
        {
            _descriptorService = descriptorService;
            _inventoryService = inventoryService;
        }

        public void PickUpItem(ItemController itemController)
        {
            string itemId = itemController.GetName;
            ItemsDescriptor itemDescriptor = _descriptorService.Require<ItemsDescriptor>();
            List<ItemDescriptorModel> items = itemDescriptor.ItemDescriptors;
            ItemDescriptorModel itemModel = items.Find(item => item.ItemId == itemId);
            if (itemModel == null) {
                throw new ArgumentException($"Item not found with id={itemId}");
            }

            if (_inventoryService.TryAddItemToInventory(itemId, itemModel.ItemType)) {
                itemController.gameObject.DestroyObject();
            }
        }
    }
}