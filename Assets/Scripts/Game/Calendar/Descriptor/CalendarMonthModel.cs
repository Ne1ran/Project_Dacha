using System;
using Game.Calendar.Model;
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
        [field: SerializeField]
        public MonthClimateSettings ClimateSettings { get; set; } = null!;
    }
}