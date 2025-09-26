using System;
using Game.Plants.Model;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantTemperatureParameters : IPlantParameters
    {
        public PlantParametersType ParametersType => PlantParametersType.Temperature;

        [field: SerializeField, Range(-40f, 45f), Tooltip("Min temperature needed for plant to grow")]
        public float Min { get; set; } = 15f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Max temperature needed for plant to grow")]
        public float Max { get; set; } = 35f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Preferred temperature for plant")]
        public float MinPreferred { get; set; } = 25f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Preferred temperature for plant")]
        public float MaxPreferred { get; set; } = 29f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 temperature deviation")]
        public float DamagePerDeviation { get; set; } = 2f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred temperature")]
        public float GrowBuff { get; set; } = 0.1f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Stress gain per temperature deviation")]
        public float StressGain { get; set; } = 1f;
    }
}