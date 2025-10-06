using System;
using Game.Weather.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class MonthClimateSettings
    {
        [field: SerializeField, Range(-10f, 10f),
                Tooltip("Min month temperature noise. Will randomise month deviation from base month temperature in control points")]
        public float MinMonthTemperatureNoise { get; set; } = -3f;
        [field: SerializeField, Range(-10f, 10f),
                Tooltip("Max month temperature noise. Will randomise month deviation from base month temperature in control points")]
        public float MaxMonthTemperatureNoise { get; set; } = 3f;
        [field: SerializeField, Range(0f, 16f),
                Tooltip("Day-Night min temperature difference. Will randomise temperature between min and max every day")]
        public float MinDiurnalTemperatureDifference { get; set; } = 4f;
        [field: SerializeField, Range(0f, 16f),
                Tooltip("Day-Night max temperature difference. Will randomise temperature between min and max every day")]
        public float MaxDiurnalTemperatureDifference { get; set; } = 8f;
        [field: SerializeField]
        public float DailyTemperatureNoise { get; set; } = 1.2f;
        [field: SerializeField]
        public float DailyTemperatureMaxNoise { get; set; } = 3f;

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
        [field: SerializeField, Range(0f, 1f), Tooltip("Chance of wind speed being randomised between min and max speed possible")]
        public float RandomWindChance { get; set; } = 0.1f;
        [field: SerializeField, Range(1f, 10f), Tooltip("Average daily wind speed in this month")]
        public float AverageWindSpeed { get; set; } = 4.5f;
        [field: SerializeField, Range(0f, 5f), Tooltip("Wind speed noise. windSpeed +- random value between -max and max")]
        public float WindSpeedMaxNoise { get; set; } = 2f;
        [field: SerializeField]
        public SerializedDictionary<WeatherType, WeatherSettings> Weather { get; set; } = new();
        [field: SerializeField]
        public SerializedDictionary<int, float> TemperatureControlPoints { get; set; } = new();
    }
}