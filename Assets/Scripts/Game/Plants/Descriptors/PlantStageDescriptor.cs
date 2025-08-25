using System;
using Game.Plants.Model;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantStageDescriptor
    {
        [field: SerializeField]
        public PlantGrowStage Stage { get; set; } = PlantGrowStage.SEED;
        [field: SerializeField]
        public float AverageGrowTime { get; set; } = 7f;
        [field: SerializeField]
        public PlantConsumptionDescriptor PlantConsumption { get; set; } = null!;
        [field: SerializeField]
        public PlantPreferredParameters PreferredParameters { get; set; } = null!;
    }
}