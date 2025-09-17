using System.Collections.Generic;
using Core.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Harvest.Descriptor
{
    [CreateAssetMenu(fileName = "PlantHarvestDescriptor", menuName = "Dacha/Descriptors/PlantHarvestDescriptor")]
    [Descriptor("Descriptors/" + nameof(PlantHarvestDescriptor))]
    public class PlantHarvestDescriptor : ScriptableObject
    {
        [field: SerializeField, TableList]
        public List<PlantHarvestDescriptorModel> Items { get; set; } = new();

        public PlantHarvestDescriptorModel? FindItemById(string id)
        {
            return Items.Find(hdm => hdm.PlantId == id);
        }
    }
}