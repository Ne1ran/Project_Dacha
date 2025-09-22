using System.Collections.Generic;
using Core.Attributes;
using Game.Calendar.Model;
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