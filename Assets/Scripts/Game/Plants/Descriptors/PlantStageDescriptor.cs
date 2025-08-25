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
        public PlantSunlightParameters SunlightParameters { get; set; } = null!;
        [field: SerializeField]
        public PlantTemperatureParameters TemperatureParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Air humidity parameters for plant")]
        public PlantHumidityParameters AirHumidityParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Soil humidity parameters for plant")]
        public PlantHumidityParameters SoilHumidityParameters { get; set; } = null!;
    }
}