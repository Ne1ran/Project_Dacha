using System;
using System.Collections.Generic;
using Game.Plants.Model;
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
        public string Id { get; set; } = null!;
        [field: SerializeField]
        public string Name { get; set; } = null!;
        [field: SerializeField]
        public int PlantsCount { get; set; } = 1;
        [field: SerializeField, Range(0f, 1f), Tooltip("Noise for plant to grow. ActualGrowth + Rnd(-value;value)")]
        public float PlantGrowNoise { get; set; } = 0.1f;
        [field: SerializeField, Tooltip("Family type of a plant")]
        public PlantFamilyType FamilyType { get; set; } = PlantFamilyType.None;
        [field: SerializeField, Tooltip("Plant ph parameters to grow")]
        public PlantPhParameters PhParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Plant stress parameters")]
        public PlantStressParameters StressParameters { get; set; } = null!;
        [field: SerializeField]
        public PlantVisualizationDescriptor Visualization { get; set; } = null!;
        
        [field: SerializeField]
        public SerializedDictionary<PlantGrowStage, PlantStageDescriptor> Stages2 { get; set; } = new();

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