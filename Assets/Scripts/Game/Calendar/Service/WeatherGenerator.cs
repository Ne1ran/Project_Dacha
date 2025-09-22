using System;
using System.Collections.Generic;
using System.Linq;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Utils;
using Game.Weather.Model;

namespace Game.Calendar.Service
{
    public sealed class WeatherGenerator
    {
        private readonly Random _random = new();

        public List<DailyWeather> GenerateMonthlyWeather(MonthClimateSettings settings)
        {
            float[] temperatures = GetDailyTemperatures(settings.TemperatureControlPoints, Constants.Constants.DaysInMonth, out float monthlyAverageTemperature);
            WeatherType[] weatherStateByDay = GetAllDaysWeather(settings.Weather, Constants.Constants.DaysInMonth);
            List<DailyWeather> results = new(Constants.Constants.DaysInMonth);

            for (int i = 0; i < Constants.Constants.DaysInMonth; i++) {
                WeatherType weatherState = weatherStateByDay[i];
                WeatherSettings weatherSettings = settings.Weather[weatherState];
                float temperatureShiftByState = weatherSettings.TemperatureShift;
                float temperatureNoise = (float) Clamp(NextGaussian(0.0, settings.TemperatureNoiseStandardDeviationCelsius),
                                                       -settings.TemperatureNoiseCapCelsius, settings.TemperatureNoiseCapCelsius);

                float dailyAverageTemperature = temperatures[i] + temperatureShiftByState + temperatureNoise;

                float diurnalAmplitude = weatherSettings.DiurnalAmplitudeTemperature;

                float dailyMaxTemperature = dailyAverageTemperature + diurnalAmplitude / 2f;
                float dailyMinTemperature = dailyAverageTemperature - diurnalAmplitude / 2f;

                float astronomicalSunHours = settings.SunHours;
                float sunHoursFactor = weatherSettings.SunHoursMultiplier;
                float sunHoursNoiseMultiplier =
                        1f + (float) Lerp(-settings.SunHoursNoise, settings.SunHoursNoise, _random.NextDouble());
                float sunHours = astronomicalSunHours * sunHoursFactor * sunHoursNoiseMultiplier;
                sunHours = (float) Clamp(sunHours, 1f, astronomicalSunHours);

                float baseRelativeHumidity = weatherSettings.AirHumidity;
                float relativeHumidityDeltaFromTemperature = (monthlyAverageTemperature - dailyAverageTemperature)
                                                             * settings.HumiditySensitivityPerCelsius;
                float relativeHumidityNoise = (float) Lerp(-settings.HumidityNoise, settings.HumidityNoise,
                                                           _random.NextDouble());

                float relativeHumidity = baseRelativeHumidity + relativeHumidityDeltaFromTemperature + relativeHumidityNoise;
                relativeHumidity = (float) Clamp(relativeHumidity, 0.1f, 0.95f);

                float precipitationMillimeters = 0f;
                if (weatherSettings.CanHavePrecipitation) {
                    float randomPrecipitationNoise =
                            (float) Lerp(-settings.PrecipitationNoise, settings.PrecipitationNoise, _random.NextDouble());
                    float precipitationNoiseMultiplier = 1f + randomPrecipitationNoise;
                    float randomPrecipitation = UnityEngine.Random.Range(weatherSettings.MinPrecipitation, weatherSettings.MaxPrecipitation);
                    precipitationMillimeters = Math.Max(0f, randomPrecipitation * precipitationNoiseMultiplier);
                }

                results.Add(new(i + 1, weatherState, dailyAverageTemperature, dailyMaxTemperature, dailyMinTemperature, sunHours, relativeHumidity,
                                precipitationMillimeters));
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
                    if (_random.NextDouble() < configWeather[lastWeather].SafeProbability) {
                        weatherArray[i] = lastWeather;
                    }
                }
                WeatherType weatherOnDay = GetWeatherOnDay(configWeather);
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

        private double NextGaussian(double mean, double stdDev)
        {
            double u1 = 1.0 - _random.NextDouble();
            double u2 = 1.0 - _random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0f * Math.Log(u1)) * Math.Sin(2.0f * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }
    }
}