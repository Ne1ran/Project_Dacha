using System;
using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Utils;
using Game.Weather.Model;
using UnityEngine;
using IInitializable = VContainer.Unity.IInitializable;
using Random = System.Random;

namespace Game.Calendar.Service
{
    [Service]
    public class TestService : IInitializable
    {
        private readonly IDescriptorService _descriptorService;

        public TestService(IDescriptorService descriptorService)
        {
            _descriptorService = descriptorService;
        }

        public void Initialize()
        {
            CalendarDescriptor calendarDescriptor = _descriptorService.Require<CalendarDescriptor>();
            CalendarMonthModel calendarMonthModel = calendarDescriptor.FindByType(MonthType.July);

            MonthlyWeatherGenerator generator = new();
            List<DailyWeatherResult> july = generator.GenerateMonthlyWeather(calendarMonthModel.ClimateSettings);

            foreach (DailyWeatherResult julyWeather in july) {
                Debug.Log($"CurrentDay={julyWeather.Date} \n" + $"Weather is {julyWeather.WeatherType.ToString()} \n"
                          + $"Temperature settings are AverageTemperatureCelsius={julyWeather.AverageTemperatureCelsius}, MinTemperatureCelsius={julyWeather.MinTemperatureCelsius}, MaxTemperatureCelsius={julyWeather.MaxTemperatureCelsius} \n"
                          + $"Other settings are SunHours={julyWeather.SunHours}, RelativeHumidity={julyWeather.RelativeHumidity}, PrecipitationMillimeters={julyWeather.PrecipitationMillimeters} \n");
            }
        }
    }

    public sealed class DailyWeatherResult
    {
        public DateTime Date;
        public WeatherType WeatherType;

        public float AverageTemperatureCelsius;
        public float MaxTemperatureCelsius;
        public float MinTemperatureCelsius;

        public float SunHours; // часы
        public float RelativeHumidity; // 0..1
        public float PrecipitationMillimeters; // мм
    }

    public sealed class MonthlyWeatherGenerator
    {
        private readonly Random random = new();

        public List<DailyWeatherResult> GenerateMonthlyWeather(MonthClimateSettings settings)
        {
            int daysInMonth = 28;

            // 1) Базовая температура по контрольным точкам + средняя по месяцу
            float[] linearTemperatureByDay =
                    BuildBaseDailyTemperatureFromControlPoints(settings.TemperatureControlPoints, daysInMonth, out float monthlyAverageTemperature);

            // 2) Генерация последовательности состояний с персистентностью
            WeatherType[] weatherStateByDay = GetAllDaysWeather(settings.Weather, daysInMonth);

            // 3) Расчёт дневных значений
            List<DailyWeatherResult> results = new(daysInMonth);

            for (int i = 0; i < daysInMonth; i++) {
                WeatherType weatherState = weatherStateByDay[i];

                // Сдвиг температуры по состоянию
                WeatherSettings weatherSettings = settings.Weather[weatherState];
                float temperatureShiftByState = weatherSettings.TemperatureShift;

                // Шум температуры (усечённый)
                float temperatureNoise = (float) Clamp(NextGaussian(0.0, settings.TemperatureNoiseStandardDeviationCelsius),
                                                       -settings.TemperatureNoiseCapCelsius, settings.TemperatureNoiseCapCelsius);

                float dailyAverageTemperature = linearTemperatureByDay[i] + temperatureShiftByState + temperatureNoise;

                // Амплитуда по состоянию
                float diurnalAmplitude = weatherSettings.DiurnalAmplitudeTemperature;

                float dailyMaxTemperature = dailyAverageTemperature + diurnalAmplitude / 2f;
                float dailyMinTemperature = dailyAverageTemperature - diurnalAmplitude / 2f;

                // Солнце
                float astronomicalSunHours = settings.AstronomicalSunHoursPerDayConstant;
                float sunHoursFactor = weatherSettings.SunHoursMultiplier;
                float sunHoursNoiseMultiplier =
                        1f + (float) Lerp(-settings.SunHoursNoiseFraction, settings.SunHoursNoiseFraction, random.NextDouble());
                float sunHours = astronomicalSunHours * sunHoursFactor * sunHoursNoiseMultiplier;
                sunHours = (float) Clamp(sunHours, 0f, astronomicalSunHours);

                // Влажность
                float baseRelativeHumidity = weatherSettings.AirHumidity;

                float relativeHumidityDeltaFromTemperature = (monthlyAverageTemperature - dailyAverageTemperature)
                                                             * settings.RelativeHumidityTemperatureSensitivityPerCelsius;

                float relativeHumidityNoise = (float) Lerp(-settings.RelativeHumidityNoiseFraction, settings.RelativeHumidityNoiseFraction,
                                                           random.NextDouble());

                float relativeHumidity = baseRelativeHumidity + relativeHumidityDeltaFromTemperature + relativeHumidityNoise;
                relativeHumidity = (float) Clamp(relativeHumidity, 0f, 1f);

                // Осадки
                float precipitationMillimeters = 0f;
                if (weatherSettings.CanHavePrecipitation) {
                    float randomPrecipitationNoise =
                            (float) Lerp(-settings.PrecipitationNoiseFraction, settings.PrecipitationNoiseFraction, random.NextDouble());
                    float precipitationNoiseMultiplier = 1f + randomPrecipitationNoise;
                    float randomPrecipitation = UnityEngine.Random.Range(weatherSettings.MinPrecipitation, weatherSettings.MaxPrecipitation);
                    precipitationMillimeters = Math.Max(0f, randomPrecipitation * precipitationNoiseMultiplier);
                }

                results.Add(new() {
                        Date = new(2025, 7, i + 1),
                        WeatherType = weatherState,
                        AverageTemperatureCelsius = dailyAverageTemperature,
                        MaxTemperatureCelsius = dailyMaxTemperature,
                        MinTemperatureCelsius = dailyMinTemperature,
                        SunHours = sunHours,
                        RelativeHumidity = relativeHumidity,
                        PrecipitationMillimeters = precipitationMillimeters
                });
            }

            return results;
        }

        private WeatherType[] GetAllDaysWeather(Dictionary<WeatherType, WeatherSettings> configWeather,
                                                int daysInMonth,
                                                WeatherType previousWeather = WeatherType.None)
        {
            WeatherType[] weatherArray = new WeatherType[daysInMonth];
            WeatherType lastWeather = previousWeather;
            for (int i = 0; i < daysInMonth; i++) {
                if (i != 0 && previousWeather != WeatherType.None) {
                    if (random.NextDouble() < configWeather[lastWeather].SafeProbability) {
                        weatherArray[i] = lastWeather;
                    }
                }
                WeatherType weatherOnDay = GetWeatherOnDay(configWeather);
                weatherArray[i] = weatherOnDay;
                lastWeather = weatherOnDay;
            }

            return weatherArray;
        }

        private float[] BuildBaseDailyTemperatureFromControlPoints(Dictionary<int, float> controlPoints,
                                                                   int daysInMonth,
                                                                   out float computedMonthlyAverageTemperature)
        {
            if (controlPoints.Count == 0) {
                throw new InvalidOperationException("TemperatureControlPointsByDay must contain at least one point.");
            }

            List<KeyValuePair<int, float>> sortedPoints = new();
            foreach ((int day, float temperature) in controlPoints) {
                if (day >= 1 && day <= daysInMonth) {
                    sortedPoints.Add(new(day, temperature));
                }
            }

            if (sortedPoints.Count == 0) {
                throw new InvalidOperationException("No valid control points within month day range.");
            }

            sortedPoints.Sort((a, b) => a.Key.CompareTo(b.Key));

            float[] temperatureByDay = new float[daysInMonth];

            int firstDay = sortedPoints[0].Key;
            float firstTemp = sortedPoints[0].Value;
            for (int d = 1; d < firstDay; d++) {
                temperatureByDay[d - 1] = firstTemp;
            }

            for (int p = 0; p < sortedPoints.Count - 1; p++) {
                int dayA = sortedPoints[p].Key;
                float tempA = sortedPoints[p].Value;
                int dayB = sortedPoints[p + 1].Key;
                float tempB = sortedPoints[p + 1].Value;

                int length = dayB - dayA;
                if (length <= 0) {
                    continue;
                }

                for (int d = dayA; d < dayB; d++) {
                    float t = (float) (d - dayA) / length;
                    float temp = tempA + (tempB - tempA) * t;
                    temperatureByDay[d - 1] = temp;
                }

                temperatureByDay[dayB - 1] = tempB;
            }

            int lastDay = sortedPoints[^1].Key;
            float lastTemp = sortedPoints[^1].Value;
            for (int d = lastDay + 1; d <= daysInMonth; d++) {
                temperatureByDay[d - 1] = lastTemp;
            }

            double sum = 0;
            for (int i = 0; i < daysInMonth; i++) {
                sum += temperatureByDay[i];
            }
            computedMonthlyAverageTemperature = (float) (sum / daysInMonth);

            return temperatureByDay;
        }

        private WeatherType GetWeatherOnDay(Dictionary<WeatherType, WeatherSettings> weatherSettings)
        {
            List<float> weights = new();
            float totalWeight = 0f;

            foreach ((WeatherType _, WeatherSettings settings) in weatherSettings) {
                totalWeight += settings.Chance;
                weights.Add(settings.Chance);
            }

            int pickedIndex = RandomUtils.PickRandomByWeights(weights, totalWeight);
            WeatherType randomWeather = weatherSettings.Keys.ToList()[pickedIndex];
            return randomWeather;
        }

        private static double Clamp(double value, double min, double max)
        {
            if (value < min) {
                return min;
            }
            if (value > max) {
                return max;
            }
            return value;
        }

        private static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        // Бокса–Мюллера для нормального шума
        private double NextGaussian(double mean, double stdDev)
        {
            // избегаем 0
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}