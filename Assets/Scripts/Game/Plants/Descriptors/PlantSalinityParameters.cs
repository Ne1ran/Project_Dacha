using System;
using Game.Plants.Model;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantSalinityParameters : IPlantParameters
    {
        public PlantParametersType ParametersType => PlantParametersType.Salinity;

        [field: SerializeField, Range(0f, 1f), Tooltip("Min salinity for plant to live")]
        public float Min { get; set; } = 0.01f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Max salinity for plant to live")]
        public float Max { get; set; } = 0.2f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Min preferred salinity for plant")]
        public float MinPreferred { get; set; } = 0.1f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Max preferred salinity for plant")]
        public float MaxPreferred { get; set; } = 0.125f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 salinity deviation")]
        public float DamagePerDeviation { get; set; } = 50f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Grow buff for preferred salinity")]
        public float GrowBuff { get; set; } = 0.05f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Stress gain per 1 salinity deviation")]
        public float StressGain { get; set; } = 100f;
    }
}