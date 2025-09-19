using System;
using System.Collections.Generic;
using Game.Plants.Model;
using UnityEngine;

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
        public string Id { get; set; } = null!;
        [field: SerializeField]
        public string Name { get; set; } = null!;
        [field: SerializeField]
        public int PlantsCount { get; set; } = 1;
        [field: SerializeField, Tooltip("Family type of a plant")]
        public PlantFamilyType FamilyType { get; set; } = PlantFamilyType.None;
        [field: SerializeField]
        public PlantVisualizationDescriptor Visualization { get; set; } = null!;
        [field: SerializeField]
        public List<PlantStageDescriptor> Stages { get; set; } = new();

        public PlantStageDescriptor RequireStage(PlantGrowStage stage)
        {
            PlantStageDescriptor? plantStageDescriptor = Stages.Find(plantStage => plantStage.Stage == stage);
            if (plantStageDescriptor == null) {
                throw new KeyNotFoundException($"Plant selected stage not found={stage}");
            }
            
            return plantStageDescriptor;
        }
    }
}