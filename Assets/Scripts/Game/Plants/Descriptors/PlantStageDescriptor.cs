using System;
using Game.Plants.Model;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantStageDescriptor
    {
        [field: SerializeField]
        public PlantGrowStage Stage { get; set; } = PlantGrowStage.SEED;
        [field: SerializeField]
        public AssetReference Prefab { get; set; } = null!;
        [field: SerializeField]
        public float AverageGrowTime { get; set; } = 7f;
        [field: SerializeField, Tooltip("Plant regeneration every day. Consumes humus to convert into health")]
        public float DailyRegeneration { get; set; } = 3f;
        [field: SerializeField, Tooltip("Plant immunity gain every day. Doesn't consume anything. Applies only if there was no stress")]
        public float DailyImmunityGain { get; set; } = 3f;

        [field: SerializeField, Tooltip("Need to do calculation in consumption?")]
        public bool IncludeConsumption { get; set; } = true;

        [field: SerializeField, Tooltip("Need to do calculation in temperature?")]
        public bool IncludeSunlight { get; set; } = true;

        [field: SerializeField, Tooltip("Need to do calculation in temperature?")]
        public bool IncludeTemperature { get; set; } = true;

        [field: SerializeField, Tooltip("Need to do calculation in temperature?")]
        public bool IncludeAirHumidity { get; set; } = true;

        [field: SerializeField, Tooltip("Need to do calculation in soil humidity?")]
        public bool IncludeSoilHumidity { get; set; } = true;

        [field: SerializeField, Tooltip("Need to do calculation in salinity?")]
        public bool IncludeSalinity { get; set; } = true;

        [field: SerializeField, Tooltip("Plant consumption parameters for plant"), ShowIf("IncludeConsumption")]
        public PlantConsumptionDescriptor PlantConsumption { get; set; } = null!;
        [field: SerializeField, Tooltip("Sun hours parameters for plant"), ShowIf("IncludeSunlight")]
        public PlantSunlightParameters SunlightParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Temperature parameters for plant"), ShowIf("IncludeTemperature")]
        public PlantTemperatureParameters TemperatureParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Air humidity parameters for plant"), ShowIf("IncludeAirHumidity")]
        public PlantHumidityParameters AirHumidityParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Soil humidity parameters for plant"), ShowIf("IncludeSoilHumidity")]
        public PlantHumidityParameters SoilHumidityParameters { get; set; } = null!;
        [field: SerializeField, Tooltip("Salinity parameters for plant"), ShowIf("IncludeSalinity")]
        public PlantSalinityParameters SalinityParameters { get; set; } = null!;
    }
}