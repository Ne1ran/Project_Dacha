using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class WeatherSettings
    {
        [field: SerializeField, Range(0f, 1f)]
        public float Chance { get; set; }
        [field: SerializeField, Range(-10f, 10f)]
        public float TemperatureShift { get; set; }
        [field: SerializeField, Range(0f, 1f)]
        public float AirHumidity { get; set; }
        [field: SerializeField, Range(0f, 1f)]
        public float SafeProbability { get; set; }
        [field: SerializeField, Range(0f, 1f)]
        public float SunHoursMultiplier { get; set; }
        [field: SerializeField]
        public float DiurnalAmplitudeTemperature { get; set; }
        [field: SerializeField]
        public bool CanHavePrecipitation { get; set; }
        [field: SerializeField, Range(0f, 6f), ShowIf("CanHavePrecipitation")]
        public float MinPrecipitation { get; set; }
        [field: SerializeField, Range(0f, 24f), ShowIf("CanHavePrecipitation")]
        public float MaxPrecipitation { get; set; } = 8f;
    }
}