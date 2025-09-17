using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Harvest.Component;
using Game.Harvest.Descriptor;
using Game.Inventory.Model;
using Game.Inventory.Service;
using Game.Items.Descriptors;
using Game.Plants.Model;
using Game.Plants.Service;
using UnityEngine;

namespace Game.Harvest.Service
{
    [Service]
    public class PlantHarvestService
    {
        private readonly PlantsService _plantsService;
        private readonly InventoryService _inventoryService;
        private readonly IDescriptorService _descriptorService;
        private readonly IResourceService _resourceService;

        public PlantHarvestService(PlantsService plantsService, InventoryService inventoryService, IDescriptorService descriptorService, IResourceService resourceService)
        {
            _plantsService = plantsService;
            _inventoryService = inventoryService;
            _descriptorService = descriptorService;
            _resourceService = resourceService;
        }

        public async UniTask<HarvestController> CreateHarvest(string itemId, Vector3 position)
        {
            ItemsDescriptor itemsDescriptor = _descriptorService.Require<ItemsDescriptor>();
            List<ItemDescriptorModel> items = itemsDescriptor.ItemDescriptors;
            ItemDescriptorModel? itemDescriptorModel = items.Find(item => item.ItemId == itemId);
            if (itemDescriptorModel == null) {
                throw new ArgumentException($"Item not found with id={itemId}");
            }

            HarvestController harvestController = await _resourceService.LoadObjectAsync<HarvestController>(itemDescriptorModel.ItemPrefab!);
            harvestController.transform.position = position;
            harvestController.name = itemId;
            return harvestController;
        }

        public bool TryHarvestPlant(string plantTileId)
        {
            PlantModel? plantModel = _plantsService.GetPlant(plantTileId);
            if (plantModel == null) {
                Debug.LogWarning($"Plant not found on tile={plantTileId}");
                return false;
            }

            PlantHarvestDescriptor plantHarvestDescriptor = _descriptorService.Require<PlantHarvestDescriptor>();
            PlantHarvestDescriptorModel? harvestModel = plantHarvestDescriptor.FindItemById(plantModel.PlantId);
            if (harvestModel == null) {
                Debug.LogWarning($"Plant harvest descriptor not found on tile={plantTileId} for plant={plantModel.PlantId}");
                return false;
            }

            return _inventoryService.TryAddItemToInventory(harvestModel.HarvestItemId, ItemType.HARVEST);
        }
    }
}