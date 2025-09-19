using System;
using System.Collections.Generic;
using Game.Items.Descriptors;
using Game.Plants.Descriptors;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Harvest.Descriptor
{
    [Serializable]
    public class PlantHarvestDescriptorModel
    {
        [field: SerializeField, ValueDropdown("GetPlantIds")]
        public string Id { get; set; } = null!;
        [field: SerializeField]
        public float BaseHarvestRate { get; set; } = 1f;
        [field: SerializeField]
        public float MultiplierPerHealth { get; set; } = 0.02f;
        // todo neiran add additional parameters that influence harvest rate.

        [field: SerializeField, ValueDropdown("GetItemsIds")]
        public string HarvestItemId { get; set; } = null!;

        public List<string> GetPlantIds()
        {
            PlantsDescriptor plantsDescriptor = Resources.Load<PlantsDescriptor>("Descriptors/PlantsDescriptor");
            List<string> result = new();
            foreach (PlantsDescriptorModel pdm in plantsDescriptor.Items) {
                result.Add(pdm.Id);
            }

            return result;
        }

        public List<string> GetItemsIds()
        {
            ItemsDescriptor plantsDescriptor = Resources.Load<ItemsDescriptor>("Descriptors/ItemsDescriptor");
            List<string> result = new();
            foreach (ItemDescriptorModel item in plantsDescriptor.Items) {
                result.Add(item.Id);
            }

            return result;
        }
    }
}