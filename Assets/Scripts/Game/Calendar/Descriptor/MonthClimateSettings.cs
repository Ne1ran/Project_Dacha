using System;
using Game.Weather.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class MonthClimateSettings
    {
        [field: SerializeField]
        public float TemperatureNoise { get; set; } = 1.2f;
        [field: SerializeField]
        public float TemperatureMaxNoise { get; set; } = 3f;
        
        [field: SerializeField, Range(0f, 1f), Tooltip("Sunlight noise. +- random value * actual sun hours * other multipliers")]
        public float SunHoursNoise { get; set; } = 0.1f;
        [field: SerializeField, Range(0f, 0.25f), Tooltip("Humidity noise. +- random value + actual humidity")]
        public float HumidityNoise { get; set; } = 0.05f;
        [field: SerializeField, Range(0f, 1f), Tooltip("Precipitation noise. +- random value * actual precipitation")]
        public float PrecipitationNoise { get; set; } = 0.25f;
        
        [field: SerializeField, Range(0f, 0.1f), Tooltip("Humidity dependence on difference between current and average temperature")]
        public float HumiditySensitivityPerCelsius { get; set; } = 0.025f;
        [field: SerializeField, Range(0f, 24f), Tooltip("Amount of sun hours every day in month")]
        public float SunHours { get; set; } = 12f;

        [field: SerializeField]
        public SerializedDictionary<WeatherType, WeatherSettings> Weather { get; set; } = new();
        [field: SerializeField]
        public SerializedDictionary<int, float> TemperatureControlPoints { get; set; } = new();
    }
}