using System;
using UnityEngine;

namespace Game.Evaporation.Descriptor
{
    [Serializable]
    public class WaterEvaporationSettings
    {
        // Weather of months
        [field: SerializeField, Tooltip("Normal amount of sun hours as reference for evaporation")]
        public float NormalSunHours { get; set; } = 12f; 
        [field: SerializeField, Tooltip("Month average solar radiation")]
        public float SolarRadiation { get; set; } = 150f; 

        // Soil and calculations
        [field: SerializeField, Range(0f, 100f), Tooltip("Wilt point of water in soil to start decreasing of evaporation")]
        public float WiltPoint { get; set; } = 10f;
        [field: SerializeField, Range(0f, 200f), Tooltip("Water capacity of soil")]
        public float WaterCapacityUnits { get; set; } = 100f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Water evaporation limiter multiplier. Applies when wilt point occured")]
        public float EvaporationLimiterValue { get; set; } = 0.7f;
        [field: SerializeField, Range(0f, 10f), Tooltip("Actual amount of water that evaporates for 1 point of real evaporation")]
        public float ActualSoilEvaporationMultiplier { get; set; } = 1f;
    }
}