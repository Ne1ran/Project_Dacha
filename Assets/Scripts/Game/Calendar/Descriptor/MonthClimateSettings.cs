using System;
using Game.Weather.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class MonthClimateSettings
    {
        
        // Шумы
        [field: SerializeField]
        public float TemperatureNoiseStandardDeviationCelsius { get; set; } = 1.2f;
        [field: SerializeField]
        public float TemperatureNoiseCapCelsius { get; set; } = 3f;
        
        [field: SerializeField]
        public float SunHoursNoiseFraction { get; set; } = 0.1f;
        [field: SerializeField]
        public float RelativeHumidityNoiseFraction { get; set; } = 0.05f;
        [field: SerializeField]
        public float PrecipitationNoiseFraction { get; set; } = 0.5f;
        
        // Простая термокоррекция влажности: при дне теплее нормы влажность немного ниже
        [field: SerializeField]
        public float RelativeHumidityTemperatureSensitivityPerCelsius { get; set; } = 0.025f;
        // Астрономические солнечные часы
        [field: SerializeField]
        public float AstronomicalSunHoursPerDayConstant { get; set; } = 15f;
        
        
        [field: SerializeField]
        public SerializedDictionary<WeatherType, WeatherSettings> Weather { get; set; } = new();
        [field: SerializeField]
        public SerializedDictionary<int, float> TemperatureControlPoints { get; set; } = new();
    }
}