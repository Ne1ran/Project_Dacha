using System.Collections.Generic;
using Core.Attributes;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Difficulty.Model;
using Game.Weather.Model;
using UnityEngine;

namespace Game.Calendar.Service
{
    [Service]
    public class CalendarGenerationService
    {
        private readonly CalendarDescriptor _calendarDescriptor;

        public CalendarGenerationService(CalendarDescriptor calendarDescriptor)
        {
            _calendarDescriptor = calendarDescriptor;
        }

        public List<CalendarDayWeather> GenerateCalendarForMonth(MonthType month)
        {
            // ReSharper disable once ConvertToConstant.Local
            DachaPlaceType difficulty = DachaPlaceType.Middle; // todo neiran implement difficulty system
            CalendarMonthModel calendarMonthModel = _calendarDescriptor.FindByType(difficulty, month);
            WeatherGenerator generator = new();
            return generator.GenerateMonthlyWeather(calendarMonthModel.ClimateSettings, calendarMonthModel.DaysCount);
        }

        public void Simulate(MonthType monthType, int times)
        {
            float avgWeatherTemp = 0;
            int sunnyCounter = 0;
            int hailCounter = 0;
            int heavyRainCounter = 0;
            int rainCounter = 0;
            int lightRainCounter = 0;
            int partlyCloudyCounter = 0;
            int cloudyCounter = 0;
            int snowCounter = 0;

            float precipitations = 0;
            float windSpeedSum = 0f;
            float temperatureSum = 0f;
            int days = 0;

            CalendarMonthModel calendarMonthModel = _calendarDescriptor.FindByType(DachaPlaceType.Middle, monthType);

            for (int i = 0; i < times; i++) {
                WeatherGenerator generator = new();
                List<CalendarDayWeather> month = generator.GenerateMonthlyWeather(calendarMonthModel.ClimateSettings, calendarMonthModel.DaysCount);
                foreach (CalendarDayWeather dailyWeather in month) {
                    days++;
                    Debug.Log($"CurrentDay={dailyWeather.Day} \n" + $"Weather is {dailyWeather.WeatherType.ToString()} \n"
                              + $"Temperature settings are AverageTemperatureCelsius={dailyWeather.AverageTemperature}, "
                              + $"MinTemperatureCelsius={dailyWeather.NightTemperature}, MaxTemperatureCelsius={dailyWeather.DayTemperature} \n"
                              + $"Other settings are SunHours={dailyWeather.SunHours}, RelativeHumidity={dailyWeather.RelativeHumidity},"
                              + $" PrecipitationMillimeters={dailyWeather.Precipitations}, \n WindSpeed={dailyWeather.WindSpeed}, \n WindType={dailyWeather.WindType}");

                    switch (dailyWeather.WeatherType) {
                        case WeatherType.Sunny:
                            sunnyCounter++;
                            break;
                        case WeatherType.Hail:
                            hailCounter++;
                            break;
                        case WeatherType.PartlyCloudy:
                            partlyCloudyCounter++;
                            break;
                        case WeatherType.Cloudy:
                            cloudyCounter++;
                            break;
                        case WeatherType.LightRain:
                            lightRainCounter++;
                            break;
                        case WeatherType.Rain:
                            rainCounter++;
                            break;
                        case WeatherType.HeavyRain:
                            heavyRainCounter++;
                            break;
                        case WeatherType.Snow:
                            snowCounter++;
                            break;
                    }

                    temperatureSum += dailyWeather.AverageTemperature;
                    precipitations += dailyWeather.Precipitations;
                    windSpeedSum += dailyWeather.WindSpeed;
                }
            }

            int daysPassed = days;
            avgWeatherTemp = temperatureSum / daysPassed;
            float precipitationsPerDay = precipitations / daysPassed;
            float windSpeedPerDay = windSpeedSum / daysPassed;
            Debug.LogWarning($"Summary results for month={monthType.ToString()}: \n Days passed = {daysPassed} \n Snow days = {snowCounter} \n Sunny days = {sunnyCounter} \n "
                             + $"PartlyCloudy days = {partlyCloudyCounter} \n Cloudy days = {cloudyCounter} \n "
                             + $"Light rain days = {lightRainCounter} \n Rain days = {rainCounter} \n Heavy rain days = {heavyRainCounter} \n "
                             + $"Hail days = {hailCounter} \n Avg tempo = {avgWeatherTemp} \n Total precipitations = {precipitations} \n "
                             + $"Precipitations per day = {precipitationsPerDay} \n WindSpeedAvg={windSpeedPerDay}");
        }

        public void SimulateAll(int times)
        {
            Simulate(MonthType.January, times);
            Simulate(MonthType.February, times);
            Simulate(MonthType.March, times);
            Simulate(MonthType.April, times);
            Simulate(MonthType.May, times);
            Simulate(MonthType.June, times);
            Simulate(MonthType.July, times);
            Simulate(MonthType.August, times);
            Simulate(MonthType.September, times);
            Simulate(MonthType.October, times);
            Simulate(MonthType.November, times);
            Simulate(MonthType.December, times);
        }

        public void SimulateYear()
        {
            float globalavgWeatherTemp = 0;
            int globalsunnyCounter = 0;
            int globalhailCounter = 0;
            int globalheavyRainCounter = 0;
            int globalrainCounter = 0;
            int globallightRainCounter = 0;
            int globalpartlyCloudyCounter = 0;
            int globalcloudyCounter = 0;
            int globalsnowCounter = 0;
            float globalprecipitations = 0;
            float globalWindSpeed = 0;
            int globalDays = 0;

            for (int i = 1; i < 13; i++) {
                float avgWeatherTemp = 0;
                int sunnyCounter = 0;
                int hailCounter = 0;
                int heavyRainCounter = 0;
                int rainCounter = 0;
                int lightRainCounter = 0;
                int partlyCloudyCounter = 0;
                int cloudyCounter = 0;
                int snowCounter = 0;
                float precipitations = 0;
                float windSpeedSum = 0;

                float temperatureSum = 0f;
                int days = 0;
                MonthType monthType = (MonthType) i;
                CalendarMonthModel calendarMonthModel = _calendarDescriptor.FindByType(DachaPlaceType.Middle, monthType);

                WeatherGenerator generator = new();
                List<CalendarDayWeather> month = generator.GenerateMonthlyWeather(calendarMonthModel.ClimateSettings, calendarMonthModel.DaysCount);
                foreach (CalendarDayWeather dailyWeather in month) {
                    days++;
                    Debug.Log($"CurrentDay={dailyWeather.Day} \n" + $"Weather is {dailyWeather.WeatherType.ToString()} \n"
                              + $"Temperature settings are AverageTemperatureCelsius={dailyWeather.AverageTemperature}, MinTemperatureCelsius={dailyWeather.NightTemperature}, MaxTemperatureCelsius={dailyWeather.DayTemperature} \n"
                              + $"Other settings are SunHours={dailyWeather.SunHours}, RelativeHumidity={dailyWeather.RelativeHumidity}, PrecipitationMillimeters={dailyWeather.Precipitations} \n");

                    switch (dailyWeather.WeatherType) {
                        case WeatherType.Sunny:
                            sunnyCounter++;
                            break;
                        case WeatherType.Hail:
                            hailCounter++;
                            break;
                        case WeatherType.PartlyCloudy:
                            partlyCloudyCounter++;
                            break;
                        case WeatherType.Cloudy:
                            cloudyCounter++;
                            break;
                        case WeatherType.LightRain:
                            lightRainCounter++;
                            break;
                        case WeatherType.Rain:
                            rainCounter++;
                            break;
                        case WeatherType.HeavyRain:
                            heavyRainCounter++;
                            break;
                        case WeatherType.Snow:
                            snowCounter++;
                            break;
                    }

                    temperatureSum += dailyWeather.AverageTemperature;
                    precipitations += dailyWeather.Precipitations;
                    windSpeedSum += dailyWeather.WindSpeed;
                }

                avgWeatherTemp = temperatureSum / days;
                float precipitationsPerDay = precipitations / days;
                float windSpeedPerDay = windSpeedSum / days;
                Debug.LogWarning($"Month Summary results: \n Days passed = {days} \n Snow days = {snowCounter} \n Sunny days = {sunnyCounter} \n "
                                 + $"PartlyCloudy days = {partlyCloudyCounter} \n Cloudy days = {cloudyCounter} \n "
                                 + $"Light rain days = {lightRainCounter} \n Rain days = {rainCounter} \n Heavy rain days = {heavyRainCounter} \n "
                                 + $"Hail days = {hailCounter} \n Avg tempo = {avgWeatherTemp} \n Total precipitations = {precipitations} \n "
                                 + $"Precipitations per day = {precipitationsPerDay}, \n WindSpeed per day={windSpeedPerDay} ");

                globalavgWeatherTemp += avgWeatherTemp;
                globalsunnyCounter += sunnyCounter;
                globalhailCounter += hailCounter;
                globalheavyRainCounter += heavyRainCounter;
                globalrainCounter += rainCounter;
                globallightRainCounter += lightRainCounter;
                globalpartlyCloudyCounter += partlyCloudyCounter;
                globalcloudyCounter += cloudyCounter;
                globalsnowCounter += snowCounter;
                globalprecipitations += precipitations;
                globalWindSpeed += windSpeedSum;
                globalDays += days;
            }

            int globalDaysPassed = globalDays;
            float globalYearTemperature = globalavgWeatherTemp / 12f;
            float globalPrecipitationsPerDay = globalprecipitations / globalDaysPassed;
            float globalWindPerDay = globalWindSpeed / globalDaysPassed;
            Debug.LogWarning($"Year Summary results: \n Days passed = {globalDaysPassed} \n Snow days = {globalsnowCounter} \n Sunny days = {globalsunnyCounter} \n "
                             + $"PartlyCloudy days = {globalpartlyCloudyCounter} \n Cloudy days = {globalcloudyCounter} \n "
                             + $"Light rain days = {globallightRainCounter} \n Rain days = {globalrainCounter} \n Heavy rain days = {globalheavyRainCounter} \n "
                             + $"Hail days = {globalhailCounter} \n Avg tempo = {globalYearTemperature} \n Total precipitations = {globalprecipitations} \n "
                             + $"Precipitations per day = {globalPrecipitationsPerDay}, \n WindSpeed per day = {globalWindPerDay} ");
        }

        public void SimulateYears(int times)
        {
            float globalavgWeatherTemp = 0;
            int globalsunnyCounter = 0;
            int globalhailCounter = 0;
            int globalheavyRainCounter = 0;
            int globalrainCounter = 0;
            int globallightRainCounter = 0;
            int globalpartlyCloudyCounter = 0;
            int globalcloudyCounter = 0;
            int globalsnowCounter = 0;
            float globalprecipitations = 0;
            float globalWindSpeed = 0;

            int globalDays = 0;
            for (int i = 0; i < times; i++) {
                for (int i1 = 1; i1 < 13; i1++) {
                    float avgWeatherTemp = 0;
                    int sunnyCounter = 0;
                    int hailCounter = 0;
                    int heavyRainCounter = 0;
                    int rainCounter = 0;
                    int lightRainCounter = 0;
                    int partlyCloudyCounter = 0;
                    int cloudyCounter = 0;
                    int snowCounter = 0;
                    float precipitations = 0;
                    float windSpeedSum = 0;

                    float temperatureSum = 0f;
                    int days = 0;

                    MonthType monthType = (MonthType) i1;
                    CalendarMonthModel calendarMonthModel = _calendarDescriptor.FindByType(DachaPlaceType.Middle, monthType);

                    WeatherGenerator generator = new();
                    List<CalendarDayWeather> month =
                            generator.GenerateMonthlyWeather(calendarMonthModel.ClimateSettings, calendarMonthModel.DaysCount);
                    foreach (CalendarDayWeather dailyWeather in month) {
                        days++;
                        Debug.Log($"CurrentDay={dailyWeather.Day} \n" + $"Weather is {dailyWeather.WeatherType.ToString()} \n"
                                  + $"Temperature settings are AverageTemperatureCelsius={dailyWeather.AverageTemperature}, MinTemperatureCelsius={dailyWeather.NightTemperature}, MaxTemperatureCelsius={dailyWeather.DayTemperature} \n"
                                  + $"Other settings are SunHours={dailyWeather.SunHours}, RelativeHumidity={dailyWeather.RelativeHumidity}, PrecipitationMillimeters={dailyWeather.Precipitations} \n");

                        switch (dailyWeather.WeatherType) {
                            case WeatherType.Sunny:
                                sunnyCounter++;
                                break;
                            case WeatherType.Hail:
                                hailCounter++;
                                break;
                            case WeatherType.PartlyCloudy:
                                partlyCloudyCounter++;
                                break;
                            case WeatherType.Cloudy:
                                cloudyCounter++;
                                break;
                            case WeatherType.LightRain:
                                lightRainCounter++;
                                break;
                            case WeatherType.Rain:
                                rainCounter++;
                                break;
                            case WeatherType.HeavyRain:
                                heavyRainCounter++;
                                break;
                            case WeatherType.Snow:
                                snowCounter++;
                                break;
                        }

                        temperatureSum += dailyWeather.AverageTemperature;
                        precipitations += dailyWeather.Precipitations;
                        windSpeedSum += dailyWeather.WindSpeed;
                    }

                    avgWeatherTemp = temperatureSum / days;
                    globalavgWeatherTemp += avgWeatherTemp;
                    globalsunnyCounter += sunnyCounter;
                    globalhailCounter += hailCounter;
                    globalheavyRainCounter += heavyRainCounter;
                    globalrainCounter += rainCounter;
                    globallightRainCounter += lightRainCounter;
                    globalpartlyCloudyCounter += partlyCloudyCounter;
                    globalcloudyCounter += cloudyCounter;
                    globalsnowCounter += snowCounter;
                    globalprecipitations += precipitations;
                    globalDays += days;
                    globalWindSpeed += windSpeedSum;
                }
            }

            float globalYearTemperature = globalavgWeatherTemp / (12f * times);
            float globalPrecipitationsPerDay = globalprecipitations / globalDays;
            float globalWindPerDay = globalWindSpeed / globalDays;
            Debug.LogWarning($"Year Summary results: \n Days passed = {globalDays} \n Snow days = {globalsnowCounter} \n Sunny days = {globalsunnyCounter} \n "
                             + $"PartlyCloudy days = {globalpartlyCloudyCounter} \n Cloudy days = {globalcloudyCounter} \n "
                             + $"Light rain days = {globallightRainCounter} \n Rain days = {globalrainCounter} \n Heavy rain days = {globalheavyRainCounter} \n "
                             + $"Hail days = {globalhailCounter} \n Avg tempo = {globalYearTemperature} \n Total precipitations = {globalprecipitations} \n "
                             + $"Precipitations per day = {globalPrecipitationsPerDay}, \n WindSpeed avg={globalWindPerDay}");
        }
    }
}