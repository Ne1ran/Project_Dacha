using System;
using Game.Plants.Model;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantSunlightParameters : IPlantParameters
    {
        public PlantParametersType ParametersType => PlantParametersType.Sunlight;

        [field: SerializeField, Range(1f, 24f), Tooltip("Min hours of sunlight needed for plant to grow")]
        public float Min { get; set; } = 6f;
        [field: SerializeField, Range(1f, 24f), Tooltip("Min hours of sunlight needed for plant to grow")]
        public float Max { get; set; } = 12f;
        [field: SerializeField, Range(1f, 24f), Tooltip("Max hours of sunlight needed for plant to grow")]
        public float MinPreferred { get; set; } = 10f;
        [field: SerializeField, Range(1f, 24f), Tooltip("Max hours of sunlight needed for plant to grow")]
        public float MaxPreferred { get; set; } = 12f;
        [field: SerializeField, Range(0f, 10f), Tooltip("Damage per 1 hour of sunlight deviation")]
        public float DamagePerDeviation { get; set; } = 2f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred sunlight")]
        public float GrowBuff { get; set; } = 0.05f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Stress gain for sunlight deviation")]
        public float StressGain { get; set; } = 1f;
    }
}