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
        [field: SerializeField, Tooltip("Plant regeneration every day. Consumes humus to convert into health")]
        public float DailyRegeneration { get; set; } = 3f;
        [field: SerializeField, Tooltip("Plant immunity gain every day. Doesn't consume anything. Applies only if there was no debuffs on plant health or growth via external factors (deceases doesn't count). Decreases proportionally with health of plant")]
        public float DailyImmunityGain { get; set; } = 3f;
        [field: SerializeField]
        public PlantConsumptionDescriptor PlantConsumption { get; set; } = null!;
        [field: SerializeField]
        public PlantSunlightParameters SunlightParameters { get; set; } = null!;
        [field: SerializeField]
        public PlantTemperatureParameters TemperatureParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Air humidity parameters for plant")]
        public PlantHumidityParameters AirHumidityParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Soil humidity parameters for plant")]
        public PlantHumidityParameters SoilHumidityParameters { get; set; } = null!;
    }
}