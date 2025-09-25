using System.Collections.Generic;
using Core.Attributes;
using Game.Calendar.Model;
using Game.Calendar.Repo;

namespace Game.Calendar.Service
{
    [Service]
    public class CalendarService
    {
        private readonly CalendarGenerationService _calendarGenerationService;
        private readonly CalendarRepo _calendarRepo;

        public CalendarService(CalendarGenerationService calendarGenerationService, CalendarRepo calendarRepo)
        {
            _calendarGenerationService = calendarGenerationService;
            _calendarRepo = calendarRepo;
        }

        public float GetDaySunHours(int day, int month)
        {
            List<CalendarDayWeather> days = GetOrCreateWeather(month);
            CalendarDayWeather? selectedWeather = days.Find(weatherDay => weatherDay.Day == day);
            if (selectedWeather == null) {
                throw new KeyNotFoundException($"Selected day not found in calendar! Day={day} Month={month}");
            }

            return selectedWeather.SunHours;
        }

        private List<CalendarDayWeather> GetOrCreateWeather(int month)
        {
            if (_calendarRepo.Exists(month)) {
                return _calendarRepo.Require(month);
            }

            List<CalendarDayWeather> monthWeather = GenerateCalendar(month);
            _calendarRepo.Save(month, monthWeather);
            return monthWeather;
        }

        private List<CalendarDayWeather> GenerateCalendar(int month)
        {
            return _calendarGenerationService.GenerateCalendarForMonth((MonthType) month);
        }
    }
}