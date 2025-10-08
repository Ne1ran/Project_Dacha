using Core.Attributes;
using Core.Descriptors.Service;
using Game.Harvest.Descriptor;
using Game.Inventory.Model;
using Game.Inventory.Service;
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

        public PlantHarvestService(PlantsService plantsService,
                                   InventoryService inventoryService,
                                   IDescriptorService descriptorService)
        {
            _plantsService = plantsService;
            _inventoryService = inventoryService;
            _descriptorService = descriptorService;
        }

        public bool TryHarvestPlant(string plantTileId)
        {
            PlantModel? plantModel = _plantsService.GetPlant(plantTileId);
            if (plantModel == null) {
                Debug.LogWarning($"Plant not found on tile={plantTileId}");
                return false;
            }

            PlantHarvestDescriptor plantHarvestDescriptor = _descriptorService.Require<PlantHarvestDescriptor>();
            PlantHarvestDescriptorModel? harvestModel = plantHarvestDescriptor.Get(plantModel.PlantId);
            if (harvestModel == null) {
                Debug.LogWarning($"Plant harvest descriptor not found on tile={plantTileId} for plant={plantModel.PlantId}");
                return false;
            }

            if (!_inventoryService.TryAddItemToInventory(harvestModel.HarvestItemId, ItemType.HARVEST)) {
                return false;
            }

            _plantsService.RemovePlant(plantTileId);
            return true;
        }
    }
}