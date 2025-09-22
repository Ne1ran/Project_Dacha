using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Difficulty.Model;
using Game.Weather.Model;
using UnityEngine;
using IInitializable = VContainer.Unity.IInitializable;

namespace Game.Calendar.Service
{
    [Service]
    public class CalendarGenerationService : IInitializable
    {
        private readonly IDescriptorService _descriptorService;

        public CalendarGenerationService(IDescriptorService descriptorService)
        {
            _descriptorService = descriptorService;
        }

        public void Initialize()
        {
            CalendarDescriptor calendarDescriptor = _descriptorService.Require<CalendarDescriptor>();
            CalendarMonthModel calendarMonthModel = calendarDescriptor.FindByType(DachaPlaceType.Middle, MonthType.July);

            WeatherGenerator generator = new();
            List<DailyWeather> july = generator.GenerateMonthlyWeather(calendarMonthModel.ClimateSettings);

            foreach (DailyWeather julyWeather in july) {
                Debug.Log($"CurrentDay={julyWeather.Day} \n" + $"Weather is {julyWeather.WeatherType.ToString()} \n"
                          + $"Temperature settings are AverageTemperatureCelsius={julyWeather.AverageTemperatureCelsius}, MinTemperatureCelsius={julyWeather.MinTemperatureCelsius}, MaxTemperatureCelsius={julyWeather.MaxTemperatureCelsius} \n"
                          + $"Other settings are SunHours={julyWeather.SunHours}, RelativeHumidity={julyWeather.RelativeHumidity}, PrecipitationMillimeters={julyWeather.PrecipitationMillimeters} \n");
            }
        }

        public void Simulate(int times)
        {
            float avgWeatherTemp = 0;
            int sunnyCounter = 0;
            int hailCounter = 0;
            int heavyRainCounter = 0;
            int rainCounter = 0;
            int lightRainCounter = 0;
            int partlyCloudyCounter = 0;
            int cloudyCounter = 0;
            float precipitations = 0;

            float temperatureSumm = 0f;
            for (int i = 0; i < times; i++) {
                CalendarDescriptor calendarDescriptor = _descriptorService.Require<CalendarDescriptor>();
                CalendarMonthModel calendarMonthModel = calendarDescriptor.FindByType(DachaPlaceType.Middle, MonthType.July);

                WeatherGenerator generator = new();
                List<DailyWeather> july = generator.GenerateMonthlyWeather(calendarMonthModel.ClimateSettings);
                foreach (DailyWeather julyWeather in july) {
                    Debug.Log($"CurrentDay={julyWeather.Day} \n" + $"Weather is {julyWeather.WeatherType.ToString()} \n"
                              + $"Temperature settings are AverageTemperatureCelsius={julyWeather.AverageTemperatureCelsius}, MinTemperatureCelsius={julyWeather.MinTemperatureCelsius}, MaxTemperatureCelsius={julyWeather.MaxTemperatureCelsius} \n"
                              + $"Other settings are SunHours={julyWeather.SunHours}, RelativeHumidity={julyWeather.RelativeHumidity}, PrecipitationMillimeters={julyWeather.PrecipitationMillimeters} \n");

                    switch (julyWeather.WeatherType) {
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
                    }

                    temperatureSumm += julyWeather.AverageTemperatureCelsius;
                    precipitations += julyWeather.PrecipitationMillimeters;
                }
            }

            int daysPassed = times * 28;
            avgWeatherTemp = temperatureSumm / daysPassed;
            float precipitationsPerDay = precipitations / daysPassed;
            Debug.LogWarning($"Summary results: \n Days passed = {daysPassed} \n Sunny days = {sunnyCounter} \n "
                             + $"PartlyCloudy days = {partlyCloudyCounter} \n Cloudy days = {cloudyCounter} \n "
                             + $"Light rain days = {lightRainCounter} \n Rain days = {rainCounter} \n Heavy rain days = {heavyRainCounter} \n "
                             + $"Hail days = {hailCounter} \n Avg tempo = {avgWeatherTemp} \n Total precipitations = {precipitations} \n "
                             + $"Precipitations per day = {precipitationsPerDay} ");
        }
    }
}