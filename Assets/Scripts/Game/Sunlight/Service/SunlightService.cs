using Core.Attributes;
using Game.Calendar.Model;
using Game.Calendar.Service;

namespace Game.Sunlight.Service
{
    [Service]
    public class SunlightService
    {
        private readonly CalendarService _calendarService;
        private readonly TimeService _timeService;

        public SunlightService(CalendarService calendarService, TimeService timeService)
        {
            _calendarService = calendarService;
            _timeService = timeService;
        }

        public float GetDailySunAmount()
        {
            TimeModel today = _timeService.GetToday();
            return _calendarService.GetDaySunHours(today.CurrentDay, today.CurrentMonth);
        }
    }
}