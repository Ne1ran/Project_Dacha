using Game.Weather.Model;

namespace Game.Calendar.Model
{
    public class DailyWeather
    {
        public int Day { get; }
        public WeatherType WeatherType { get; }
        public float AverageTemperatureCelsius { get; }
        public float MaxTemperatureCelsius { get; }
        public float MinTemperatureCelsius { get; }
        public float SunHours { get; }
        public float RelativeHumidity { get; }
        public float PrecipitationMillimeters { get; }

        public DailyWeather(int day,
                            WeatherType weatherType,
                            float averageTemperatureCelsius,
                            float maxTemperatureCelsius,
                            float minTemperatureCelsius,
                            float sunHours,
                            float relativeHumidity,
                            float precipitationMillimeters)
        {
            Day = day;
            WeatherType = weatherType;
            AverageTemperatureCelsius = averageTemperatureCelsius;
            MaxTemperatureCelsius = maxTemperatureCelsius;
            MinTemperatureCelsius = minTemperatureCelsius;
            SunHours = sunHours;
            RelativeHumidity = relativeHumidity;
            PrecipitationMillimeters = precipitationMillimeters;
        }
    }
}