using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantSalinityParameters
    {
        [field: SerializeField, Range(0f, 1f), Tooltip("Min salinity for plant to live")]
        public float MinSalinity { get; set; } = 0.01f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Max salinity for plant to live")]
        public float MaxSalinity { get; set; } = 0.2f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 salinity deviation")]
        public float DamagePerDeviation { get; set; } = 50f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Stress gain per 1 salinity deviation")]
        public float StressGain { get; set; } = 100f;
    }
}