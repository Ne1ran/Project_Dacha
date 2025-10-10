using System;
using System.Collections.Generic;
using Game.Harvest.Model;
using Game.Items.Descriptors;
using Game.Plants.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Harvest.Descriptor
{
    [Serializable]
    public class PlantHarvestDescriptorModel
    {
        [field: SerializeField, Tooltip("Type of the harvest")]
        public HarvestType HarvestType { get; set; } = HarvestType.None;
        [field: SerializeField, Tooltip("Harvest grow type")]
        public HarvestGrowType HarvestGrowType { get; set; } = HarvestGrowType.Simple;
        [field: SerializeField, Tooltip("Stage for harvest to start growing")]
        public PlantGrowStage HarvestSpawnStage { get; set; } = PlantGrowStage.REPRODUCTION;
        [field: SerializeField, Tooltip("Item id you will get when harvested"), ValueDropdown("GetItemsIds")]
        public string HarvestItemId { get; set; } = null!;
        
        [field: SerializeField, Range(0f, 10f), Tooltip("Amount of harvest that starts spawning per day")]
        public float HarvestSpawnPerDay { get; set; } = 1f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Chance of harvest to spawn per one")]
        public float HarvestSpawnChance { get; set; } = 0.5f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Multiplier for harvest spawn from health")]
        public float MultiplierPerHealth { get; set; } = 0.02f;
        [field: SerializeField, Range(1f, 5f), Tooltip("Multiplier for plant grow model")]
        public float MultiplierForGrowModel { get; set; } = 1.15f;
        [field: SerializeField, Range(1, 100), Tooltip("Point when there is too many harvest on one plant")]
        public int TooManyHarvestPoint { get; set; } = 1;
        [field: SerializeField, Range(0f, 10f), Tooltip("Debuff for harvest spawn per deviation. Formula debuff / deviation.")]
        public float TooManyHarvestDebuffPerDeviation { get; set; } = 0.5f;
        [field: SerializeField, Range(1, 100), Tooltip("Point when no harvest can spawn at all")]
        public int MaxHarvestPoint { get; set; } = 40;

        [field: SerializeField, Tooltip("Harvest grow stages")]
        public SerializedDictionary<HarvestGrowStage, PlantHarvestGrowDescriptor> HarvestGrowStages { get; set; } = new();
        
        public List<string> GetItemsIds()
        {
            ItemsDescriptor itemsDescriptor = Resources.Load<ItemsDescriptor>("Descriptors/ItemsDescriptor");
            List<string> result = new();
            foreach (string itemId in itemsDescriptor.Items.Keys) {
                result.Add(itemId);
            }

            return result;
        }
    }
}