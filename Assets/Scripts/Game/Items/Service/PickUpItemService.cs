using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Cysharp.Threading.Tasks;
using Game.Fertilizers.Service;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Controller;
using Game.Items.Descriptors;
using Game.Seeds.Service;
using Game.Tools.Service;
using Game.Utils;
using UnityEngine;

namespace Game.Items.Service
{
    [Service]
    public class PickUpItemService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly InventoryService _inventoryService;
        private readonly ToolsService _toolsService;
        private readonly FertilizerService _fertilizerService;
        private readonly SeedsService _seedsService;

        public PickUpItemService(IDescriptorService descriptorService,
                                 InventoryService inventoryService,
                                 ToolsService toolsService,
                                 FertilizerService fertilizerService,
                                 SeedsService seedsService)
        {
            _descriptorService = descriptorService;
            _inventoryService = inventoryService;
            _toolsService = toolsService;
            _fertilizerService = fertilizerService;
            _seedsService = seedsService;
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

        public async UniTask<ItemController> DropItemAsync(string itemId, ItemType itemType, Vector3 position)
        {
            switch (itemType) {
                case ItemType.TOOL:
                    return await _toolsService.CreateTool(itemId, position);
                case ItemType.FERTILIZER:
                    return await _fertilizerService.CreateFertilizer(itemId, position);
                case ItemType.SEED:
                    return await _seedsService.CreateSeedBag(itemId, position);
                default:
                    throw new ArgumentException($"Unknown item type {itemType}. Need impl");
            }
        }
    }
}