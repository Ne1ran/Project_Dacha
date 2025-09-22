using System;
using UnityEngine;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class MonthWeatherSettings
    {
        [field: SerializeField]
        public float AverageTemperature { get; set; }
        [field: SerializeField]
        public float StartTemperatureChangePerDay { get; set; }
        [field: SerializeField]
        public float EndTemperatureChangePerDay { get; set; }
        // [field: SerializeField]
        // public int StartSunlight { get; set; }
        // [field: SerializeField]
        // public int AverageSunlight { get; set; }
        // [field: SerializeField]
        // public float AverageWind { get; set; }
        // [field: SerializeField]
        // public int EndSunlight { get; set; }
        // [field: SerializeField, Range(0, 100)]
        // public int RainProbability { get; set; }
        // [field: SerializeField, Range(0, 100)]
        // public int StormProbability { get; set; }
        // [field: SerializeField, Range(0, 100)]
        // public int HailProbability { get; set; }
        // [field: SerializeField, Range(0, 100)]
        // public int HotAnomalyProbability { get; set; }
        // [field: SerializeField, Range(-10f, 0f)]
        // public float RainTemperatureChange { get; set; }
        // [field: SerializeField, Range(0f, 10f)]
        // public float HotAnomalyTemperatureChange { get; set; }
        // [field: SerializeField, Range(0f, 10f)]
        // public float StormWindChange { get; set; }
    }
}