using System;
using UnityEngine;

namespace Game.Plants.Descriptors
{
    [Serializable]
    public class PlantSunlightParameters
    {
        [field: SerializeField, Tooltip("Need to do calculation?")]
        public bool Ignore { get; set; }
        [field: SerializeField, Range(0f, 3500f), Tooltip("Min amount of sunlight needed for plant to grow")]
        public float MinSunlight { get; set; } = 500f;
        [field: SerializeField, Range(0f, 3500f), Tooltip("Max amount of sunlight needed for plant to grow")]
        public float MaxSunlight { get; set; } = 3500f;
        [field: SerializeField, Range(0f, 3500f), Tooltip("Min amount of sunlight needed for plant to grow")]
        public float MinPreferredSunlight { get; set; } = 1750f;
        [field: SerializeField, Range(0f, 3500f), Tooltip("Max amount of sunlight needed for plant to grow")]
        public float MaxPreferredSunlight { get; set; } = 2500f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Damage per 1 sunlight deviation")]
        public float DamagePerDeviation { get; set; } = 0.01f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred sunlight")]
        public float GrowBuff { get; set; } = 0.05f;
    }
}