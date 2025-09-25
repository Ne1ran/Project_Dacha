using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Difficulty.Model;
using UnityEngine;

namespace Game.Calendar.Service
{
    [Service]
    public class CalendarService
    {
        private readonly IDescriptorService _descriptorService;

        public CalendarService(IDescriptorService descriptorService)
        {
            _descriptorService = descriptorService;
        }

        public void SimulateMonth(MonthType month)
        {
            CalendarDescriptor calendarDescriptor = _descriptorService.Require<CalendarDescriptor>();
            CalendarMonthModel calendarMonth = calendarDescriptor.FindByType(DachaPlaceType.Middle, month);
            if (!calendarMonth.Playable) {
                Debug.LogWarning($"Month is not playable. MonthName={calendarMonth.Name}");
                return;
            }

            // MonthClimateSettings climateSettings = calendarMonth.ClimateSettings;
            // float currentWeatherTemperature = climateSettings.AverageTemperature;
            // for (int i = 0; i < Constants.Constants.DaysInMonth; i++) {
            //     float t = (float) i / Constants.Constants.DaysInMonth;
            //     float temperatureChange = Mathf.Lerp(climateSettings.StartTemperatureChangePerDay, climateSettings.EndTemperatureChangePerDay, t);
            //     float output = currentWeatherTemperature + temperatureChange;
            //     Debug.Log($"Day ={i + 1}. Temperature={output}");
            //     currentWeatherTemperature = output;
            // }
        }
    }
}