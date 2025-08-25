using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantHumidityParameters
    {
        [field: SerializeField, Tooltip("Need to do calculation?")]
        public bool Ignore { get; set; }
        [field: SerializeField, Range(0f, 100f), Tooltip("Min humidity for plant to survive")]
        public float MinHumidity { get; set; } = 20f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Max humidity for plant to survive")]
        public float MaxHumidity { get; set; } = 65f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Min humidity preferred by plant")]
        public float MinPreferredHumidity { get; set; } = 20f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Max humidity preferred by plant")]
        public float MaxPreferredHumidity { get; set; } = 65f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 humidity deviation")]
        public float DamagePerDeviation { get; set; } = 0.5f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Grow buff for preferred humidity")]
        public float GrowBuff { get; set; } = 0.05f;
    }
}