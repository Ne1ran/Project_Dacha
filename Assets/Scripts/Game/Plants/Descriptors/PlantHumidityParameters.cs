using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantHumidityParameters
    {
        [field: SerializeField, Range(0f, 100f), Tooltip("Min humidity for plant to survive")]
        public float MinHumidity { get; set; } = 5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Max humidity for plant to survive")]
        public float MaxHumidity { get; set; } = 65f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Min humidity preferred by plant")]
        public float MinPreferredHumidity { get; set; } = 20f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Max humidity preferred by plant")]
        public float MaxPreferredHumidity { get; set; } = 65f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 humidity deviation")]
        public float DamagePerDeviation { get; set; } = 0.25f;
        [field: SerializeField, Range(0f, 2), Tooltip("Grow buff for preferred humidity")]
        public float GrowBuff { get; set; } = 0.05f;
        [field: SerializeField, Range(0f, 2), Tooltip("Stress gain per humidity deviation")]
        public float StressGain { get; set; } = 0.1f;
    }
}