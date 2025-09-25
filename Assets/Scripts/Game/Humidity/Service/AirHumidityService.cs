using Core.Attributes;
using Game.Calendar.Model;
using Game.Calendar.Service;
using Game.Utils;

namespace Game.Humidity.Service
{
    [Service]
    public class AirHumidityService
    {
        private readonly CalendarService _calendarService;
        private readonly TimeService _timeService;

        public AirHumidityService(CalendarService calendarService, TimeService timeService)
        {
            _calendarService = calendarService;
            _timeService = timeService;
        }

        public float GetDailyAirHumidity()
        {
            TimeModel today = _timeService.GetToday();
            return _calendarService.GetAirHumidity(today.CurrentDay, today.CurrentMonth).ToPercent();
        }
    }
}