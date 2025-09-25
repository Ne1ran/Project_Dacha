using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantTemperatureParameters
    {
        [field: SerializeField, Range(-40f, 45f), Tooltip("Min temperature needed for plant to grow")]
        public float MinTemperature { get; set; } = 15f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Max temperature needed for plant to grow")]
        public float MaxTemperature { get; set; } = 35f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Preferred temperature for plant")]
        public float MinPreferredTemperature { get; set; } = 25f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Preferred temperature for plant")]
        public float MaxPreferredTemperature { get; set; } = 29f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 temperature deviation")]
        public float DamagePerDeviation { get; set; } = 2.5f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred temperature")]
        public float GrowBuff { get; set; } = 0.1f;
    }
}