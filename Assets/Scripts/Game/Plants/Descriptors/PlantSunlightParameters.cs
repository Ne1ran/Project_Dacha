using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantSunlightParameters 
    {
        [field: SerializeField, Range(1f, 24f), Tooltip("Min hours of sunlight needed for plant to grow")]
        public float MinSunlight { get; set; } = 6f;
        [field: SerializeField, Range(1f, 24f), Tooltip("Min hours of sunlight needed for plant to grow")]
        public float MaxSunlight { get; set; } = 12f;
        [field: SerializeField, Range(1f, 24f), Tooltip("Max hours of sunlight needed for plant to grow")]
        public float MinPreferredSunlight { get; set; } = 10f;
        [field: SerializeField, Range(1f, 24f), Tooltip("Max hours of sunlight needed for plant to grow")]
        public float MaxPreferredSunlight { get; set; } = 12f;
        [field: SerializeField, Range(0f, 10f), Tooltip("Damage per 1 hour of sunlight deviation")]
        public float DamagePerDeviation { get; set; } = 2f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred sunlight")]
        public float GrowBuff { get; set; } = 0.05f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Stress gain for sunlight deviation")]
        public float StressGain { get; set; } = 1f;
    }
}