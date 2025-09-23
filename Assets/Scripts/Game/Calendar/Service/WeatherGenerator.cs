using System;
using System.Collections.Generic;
using System.Linq;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Utils;
using Game.Weather.Model;
using Random = System.Random;

namespace Game.Calendar.Service
{
    public class WeatherGenerator
    {
        private readonly Random _random = new();

        public List<DailyWeather> GenerateMonthlyWeather(MonthClimateSettings settings)
        {
            float[] temperatures = GetDailyTemperatures(settings.TemperatureControlPoints, Constants.Constants.DaysInMonth,
                                                        out float monthlyAverageTemperature);
            WeatherType[] weatherStateByDay = GetAllDaysWeather(settings.Weather, temperatures, Constants.Constants.DaysInMonth);
            List<DailyWeather> results = new(Constants.Constants.DaysInMonth);

            for (int i = 0; i < Constants.Constants.DaysInMonth; i++) {
                float baseDayTemperature = temperatures[i];
                WeatherType weatherState = weatherStateByDay[i];
                WeatherSettings weatherSettings = settings.Weather[weatherState];
                float temperatureShiftByState = weatherSettings.TemperatureShift;
                float temperatureNoise = (float) Math.Clamp(NextGaussian(0f, settings.TemperatureNoise), -settings.TemperatureMaxNoise,
                                                            settings.TemperatureMaxNoise);

                float dailyAverageTemperature = baseDayTemperature + temperatureShiftByState + temperatureNoise;

                float diurnalAmplitude = weatherSettings.DiurnalAmplitudeTemperature;

                float dailyMaxTemperature = dailyAverageTemperature + diurnalAmplitude / 2f;
                float dailyMinTemperature = dailyAverageTemperature - diurnalAmplitude / 2f;

                float astronomicalSunHours = settings.SunHours;
                float sunHoursFactor = weatherSettings.SunHoursMultiplier;
                float sunHoursNoiseMultiplier = 1f + MathUtils.Lerp(-settings.SunHoursNoise, settings.SunHoursNoise, _random.NextDouble());
                float sunHours = astronomicalSunHours * sunHoursFactor * sunHoursNoiseMultiplier;
                sunHours = Math.Clamp(sunHours, 1f, astronomicalSunHours);

                float baseRelativeHumidity = weatherSettings.AirHumidity;
                float relativeHumidityDeltaFromTemperature = (monthlyAverageTemperature - dailyAverageTemperature)
                                                             * settings.HumiditySensitivityPerCelsius;
                float relativeHumidityNoise = MathUtils.Lerp(-settings.HumidityNoise, settings.HumidityNoise, _random.NextDouble());

                float relativeHumidity = baseRelativeHumidity + relativeHumidityDeltaFromTemperature + relativeHumidityNoise;
                relativeHumidity = Math.Clamp(relativeHumidity, 0.1f, 0.95f);

                float precipitationMillimeters = 0f;
                if (weatherSettings.CanHavePrecipitation) {
                    float randomPrecipitationNoise = MathUtils.Lerp(-settings.PrecipitationNoise, settings.PrecipitationNoise, _random.NextDouble());
                    float precipitationNoiseMultiplier = 1f + randomPrecipitationNoise;
                    float randomPrecipitation = UnityEngine.Random.Range(weatherSettings.MinPrecipitation, weatherSettings.MaxPrecipitation);
                    precipitationMillimeters = Math.Max(0f, randomPrecipitation * precipitationNoiseMultiplier);
                }

                WeatherType validatedWeather = ValidateSelectedWeather(weatherState, baseDayTemperature, settings);
                results.Add(new(i + 1, validatedWeather, dailyAverageTemperature, dailyMaxTemperature, dailyMinTemperature, sunHours,
                                relativeHumidity, precipitationMillimeters));
            }

            return results;
        }

        private WeatherType[] GetAllDaysWeather(Dictionary<WeatherType, WeatherSettings> configWeather,
                                                float[] temperatures,
                                                int daysInMonth,
                                                WeatherType previousWeather = WeatherType.None)
        {
            WeatherType[] weatherArray = new WeatherType[daysInMonth];
            WeatherType lastWeather = previousWeather;
            for (int i = 0; i < daysInMonth; i++) {
                if (i != 0 && previousWeather != WeatherType.None) {
                    if (_random.NextDouble() < configWeather[lastWeather].SafeProbability) {
                        weatherArray[i] = lastWeather;
                    }
                }

                float dayBaseTemperature = temperatures[i];
                WeatherType weatherOnDay = GetWeatherOnDay(configWeather, dayBaseTemperature);
                weatherArray[i] = weatherOnDay;
                lastWeather = weatherOnDay;
            }

            return weatherArray;
        }

        private float[] GetDailyTemperatures(Dictionary<int, float> controlPoints, int daysInMonth, out float computedMonthlyAverageTemperature)
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

        private WeatherType GetWeatherOnDay(Dictionary<WeatherType, WeatherSettings> weatherSettings, float dayBaseTemperature)
        {
            List<float> weights = new();
            float totalWeight = 0f;

            Dictionary<WeatherType, float> weatherChances = GetWeatherChances(weatherSettings, dayBaseTemperature);
            foreach ((WeatherType _, float chance) in weatherChances) {
                totalWeight += chance;
                weights.Add(chance);
            }

            int pickedIndex = RandomUtils.PickRandomByWeights(weights, totalWeight);
            WeatherType randomWeather = weatherChances.Keys.ToList()[pickedIndex];
            return randomWeather;
        }

        private Dictionary<WeatherType, float> GetWeatherChances(Dictionary<WeatherType, WeatherSettings> weatherSettings, float dayBaseTemperature)
        {
            Dictionary<WeatherType, float> weatherChances = new();
            foreach ((WeatherType weatherType, WeatherSettings settings) in weatherSettings) {
                switch (weatherType) {
                    case WeatherType.Snow: {
                        if (dayBaseTemperature < 0f) {
                            weatherChances.Add(weatherType, settings.Chance);
                        }

                        continue;
                    }
                    case WeatherType.LightRain or WeatherType.Rain or WeatherType.HeavyRain:
                        if (dayBaseTemperature > 0f) {
                            weatherChances.Add(weatherType, settings.Chance);
                        }
                        continue;
                    default: {
                        weatherChances.Add(weatherType, settings.Chance);
                        break;
                    }
                }
            }
            
            return weatherChances;
        }

        private WeatherType ValidateSelectedWeather(WeatherType weatherType, float baseDailyTemperature, MonthClimateSettings settings)
        {
            return weatherType switch {
                    WeatherType.Snow => baseDailyTemperature < 0f ? weatherType : settings.Weather.ContainsKey(WeatherType.Rain) ? WeatherType.Rain
                                        : WeatherType.Sunny,
                    WeatherType.Rain or WeatherType.LightRain or WeatherType.HeavyRain => baseDailyTemperature > 0f ? weatherType
                                                                                                  : settings.Weather.ContainsKey(WeatherType.Snow)
                                                                                                          ? WeatherType.Snow : WeatherType.Sunny,
                    _ => weatherType
            };
        }

        private double NextGaussian(double mean, double stdDev)
        {
            double u1 = 1.0 - _random.NextDouble();
            double u2 = 1.0 - _random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0f * Math.Log(u1)) * Math.Sin(2.0f * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}