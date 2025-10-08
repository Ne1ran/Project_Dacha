using Core.Attributes;
using Core.Descriptors.Descriptor;
using Game.Calendar.Model;
using Game.Difficulty.Model;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Calendar.Descriptor
{
    [CreateAssetMenu(fileName = "CalendarDescriptor", menuName = "Dacha/Descriptors/CalendarDescriptor")]
    [Descriptor("Descriptors/" + nameof(CalendarDescriptor))]
    public class CalendarDescriptor : Descriptor<DachaPlaceType, SerializedDictionary<MonthType, CalendarMonthModel>>
    {
        public CalendarMonthModel FindByType(DachaPlaceType placeType, MonthType monthType)
        {
            return Require(placeType)[monthType];
        }
    }
}