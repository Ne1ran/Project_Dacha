using System.Collections.Generic;
using Core.Attributes;
using Game.Calendar.Model;
using Game.Difficulty.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Calendar.Descriptor
{
    [CreateAssetMenu(fileName = "CalendarDescriptor", menuName = "Dacha/Descriptors/CalendarDescriptor")]
    [Descriptor("Descriptors/" + nameof(CalendarDescriptor))]
    public class CalendarDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<CalendarPlaceDescriptor> Items { get; set; } = new();

        public CalendarMonthModel FindByType(DachaPlaceType placeType, MonthType monthType)
        {
            CalendarPlaceDescriptor? calendarPlaceDescriptor = Items.Find(item => item.PlaceType == placeType);
            if (calendarPlaceDescriptor == null) {
                throw new KeyNotFoundException($"Calendar place descriptor not found with placeType={placeType} and monthType={monthType}");
            }
            
            return calendarPlaceDescriptor.FindByType(monthType);
        }
    }
}