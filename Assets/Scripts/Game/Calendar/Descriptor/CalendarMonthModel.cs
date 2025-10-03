using System;
using Game.Calendar.Model;
using Game.Evaporation.Descriptor;
using UnityEngine;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class CalendarMonthModel
    {
        [field: SerializeField]
        public MonthType Month { get; set; }
        [field: SerializeField]
        public string Name { get; set; } = string.Empty;
        [field: SerializeField]
        public bool Playable { get; set; } = true;
        [field: SerializeField, Range(0f, 31f), Tooltip("Days in month")]
        public int DaysCount { get; set; } = 30;
        [field: SerializeField]
        public WaterEvaporationSettings EvaporationSettings { get; set; } = null!;
        [field: SerializeField]
        public MonthClimateSettings ClimateSettings { get; set; } = null!;
    }
}