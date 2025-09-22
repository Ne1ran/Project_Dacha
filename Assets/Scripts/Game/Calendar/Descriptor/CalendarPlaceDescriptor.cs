using System;
using System.Collections.Generic;
using Game.Calendar.Model;
using Game.Difficulty.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Calendar.Descriptor
{
    [Serializable]
    public class CalendarPlaceDescriptor
    {
        [field: SerializeField]
        public DachaPlaceType PlaceType { get; set; }
        [field: SerializeField]
        [TableList]
        public List<CalendarMonthModel> Items { get; set; } = new();
        
        public CalendarMonthModel FindByType(MonthType monthType)
        {
            CalendarMonthModel? monthDesc = Items.Find(item => item.Month == monthType);
            if (monthDesc == null) {
                throw new KeyNotFoundException($"Item was not found with monthType {monthType.ToString()}!");
            }

            return monthDesc;
        }
    }
}