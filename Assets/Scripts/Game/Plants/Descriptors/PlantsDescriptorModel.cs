using System;
using System.Collections.Generic;
using Game.Harvest.Descriptor;
using Game.Plants.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantsDescriptorModel
    {
        [field: SerializeField,
                Tooltip("Plant family types:\n" + " Brassicaceae - Крестоцветные\n" + " Solanaceae - Паслёновые\n" + "Asteraceae - Сложноцветные\n"
                        + " Fabaceae - Бобовые\n" + " Rosaceae - Розоцветные\n" + " Poaceae - Злаковые\n" + " Liliaceae - Лилейные\n"
                        + " Grossulariaceae - Крыжовниковые\n" + " Ericaceae - Вересковые\n" + " Hydrophyllaceae - Водолистниковые\n"
                        + " Apiaceae - Зонтичные\n" + " Allioideae - Луковые\n" + " Cucurbitaceae - Бахчевые\n" + " Polygonaceae - Гречишные\n"
                        + " Amaranthaceae - Амарантовые")]
        public string Name { get; set; } = null!;
        [field: SerializeField]
        public int PlantsCount { get; set; } = 1;
        [field: SerializeField, Range(0f, 1f), Tooltip("Noise for plant to grow. ActualGrowth + Rnd(-value;value)")]
        public float PlantGrowNoise { get; set; } = 0.1f;
        [field: SerializeField, Tooltip("Family type of a plant")]
        public PlantFamilyType FamilyType { get; set; } = PlantFamilyType.None;
        [field: SerializeField, Tooltip("LifecycleType of a plant")]
        public PlantLifecycleType LifecycleType { get; set; } = PlantLifecycleType.OneYear;
        [field: SerializeField, Tooltip("Harvest descriptor id"), ValueDropdown("GetHarvestIds")]
        public string HarvestDescriptorId { get; set; } = string.Empty;
        [field: SerializeField, Tooltip("Plant ph parameters to grow")]
        public PlantPhParameters PhParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Plant stress parameters")]
        public PlantStressParameters StressParameters { get; set; } = null!;
        [field: SerializeField]
        public PlantVisualizationDescriptor Visualization { get; set; } = null!;

        [field: SerializeField]
        public SerializedDictionary<PlantGrowStage, PlantStageDescriptor> Stages { get; set; } = new();

        public List<string> GetHarvestIds()
        {
            PlantHarvestDescriptor harvestDescriptor = Resources.Load<PlantHarvestDescriptor>("Descriptors/PlantHarvestDescriptor");
            List<string> result = new();
            foreach (string itemId in harvestDescriptor.Items.Keys) {
                result.Add(itemId);
            }

            return result;
        }
    }
}