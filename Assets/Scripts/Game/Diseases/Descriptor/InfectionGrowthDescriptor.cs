using System;
using UnityEngine;

namespace Game.Diseases.Descriptor
{
    [Serializable]
    public class InfectionGrowthDescriptor
    {
        [field: SerializeField, Tooltip("Need calculations?")]
        public bool IgnoreTemperature { get; set; }
        [field: SerializeField, Range(-40f, 45f), Tooltip("Preferred temperature for infection to grow")]
        public float MinPreferredTemperature { get; set; } = 20f;
        [field: SerializeField, Range(-40f, 45f), Tooltip("Preferred temperature for infection to grow")]
        public float MaxPreferredTemperature { get; set; } = 24f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred temperature")]
        public float TemperatureGrowBuff { get; set; } = 0.15f;

        [field: SerializeField, Tooltip("Need calculations?")]
        public bool IgnoreAirHumidity { get; set; }
        [field: SerializeField, Range(0f, 100f), Tooltip("Preferred air humidity for infection to grow")]
        public float MinPreferredAirHumidity { get; set; } = 80f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Preferred air humidity for infection to grow")]
        public float MaxPreferredAirHumidity { get; set; } = 99f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred air humidity")]
        public float AirHumidityGrowBuff { get; set; } = 0.15f;

        [field: SerializeField, Tooltip("Need calculations?")]
        public bool IgnoreSoilHumidity { get; set; }
        [field: SerializeField, Range(0f, 100f), Tooltip("Preferred soil humidity for infection to grow")]
        public float MinPreferredSoilHumidity { get; set; } = 80f;
        [field: SerializeField, Range(0f, 100f), Tooltip("Preferred soil humidity for infection to grow")]
        public float MaxPreferredSoilHumidity { get; set; } = 99f;
        [field: SerializeField, Range(0f, 2f), Tooltip("Buff for preferred soil humidity")]
        public float SoilHumidityGrowBuff { get; set; } = 0.15f;
    }
}