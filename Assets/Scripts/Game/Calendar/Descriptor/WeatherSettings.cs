using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class WeatherSettings
    {
        [field: SerializeField, Range(0f, 1f), Tooltip("Chance of the weather to occur")]
        public float Chance { get; set; }
        [field: SerializeField, Range(-10f, 10f), Tooltip("How weather affect current temperature")]
        public float TemperatureShift { get; set; }
        [field: SerializeField, Range(0f, 1f), Tooltip("Air humidity if weather occured")]
        public float AirHumidity { get; set; }
        [field: SerializeField, Range(0f, 1f), Tooltip("Chance of the weather to save for the other day")]
        public float SafeProbability { get; set; }
        [field: SerializeField, Range(0f, 1f), Tooltip("Multiplier to sunlight amount")]
        public float SunHoursMultiplier { get; set; }
        [field: SerializeField, Tooltip("Additional amplitude of temperature during this weather")]
        public float AdditionalDiurnalAmplitudeTemperatureChange { get; set; }
        [field: SerializeField, Tooltip("Are precipitation possible?")]
        public bool CanHavePrecipitation { get; set; }
        [field: SerializeField, Range(0f, 24f), ShowIf("CanHavePrecipitation"), Tooltip("Min precipitations in mm")]
        public float MinPrecipitation { get; set; }
        [field: SerializeField, Range(0f, 48f), ShowIf("CanHavePrecipitation"), Tooltip("Max precipitations in mm")]
        public float MaxPrecipitation { get; set; } = 8f;
    }
}