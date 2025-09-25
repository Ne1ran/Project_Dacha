using Game.Weather.Model;

namespace Game.Calendar.Model
{
    public class CalendarDayWeather
    {
        public int Day { get; }
        public WeatherType WeatherType { get; }
        public float AverageTemperature { get; }
        public float DayTemperature { get; }
        public float NightTemperature { get; }
        public float SunHours { get; }
        public float RelativeHumidity { get; }
        public float Precipitations { get; }

        public CalendarDayWeather(int day,
                                  WeatherType weatherType,
                                  float averageTemperature,
                                  float dayTemperature,
                                  float nightTemperature,
                                  float sunHours,
                                  float relativeHumidity,
                                  float precipitations)
        {
            Day = day;
            WeatherType = weatherType;
            AverageTemperature = averageTemperature;
            DayTemperature = dayTemperature;
            NightTemperature = nightTemperature;
            SunHours = sunHours;
            RelativeHumidity = relativeHumidity;
            Precipitations = precipitations;
        }
    }
}