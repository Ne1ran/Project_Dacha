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
            string itemId = itemController.ItemId;
            ItemsDescriptor itemDescriptor = _descriptorService.Require<ItemsDescriptor>();
            ItemDescriptorModel itemModel = itemDescriptor.Require(itemId);
            if (_inventoryService.TryAddItemToInventory(itemId, itemModel.Type)) {
                itemController.gameObject.DestroyObject();
            }
        }
    }
}