using Core.Attributes;
using Game.Calendar.Descriptor;
using Game.Calendar.Model;
using Game.Calendar.Service;
using Game.Difficulty.Model;
using Game.Evaporation.Descriptor;
using Game.Humidity.Service;
using Game.Sunlight.Service;
using Game.Temperature.Service;
using Game.Weather.Model;
using UnityEngine;

namespace Game.Evaporation.Service
{
    [Service]
    public class SoilWaterService
    {
        private const float TurcTemperatureScaleC = 15f; // Turc formula: (T / (T + 15))
        private const float TurcCoefficient = 0.013f; // Turc coeff
        private const float RadiationToWaterScale = 23.9f; // Equivalent water in Turc's formulas
        private const float HumidityDeficitWeight = 0.4f; // Weight of water deficit (1 - RH)

        private readonly SunlightService _sunlightService;
        private readonly TemperatureService _temperatureService;
        private readonly AirHumidityService _airHumidityService;
        private readonly TimeService _timeService;
        private readonly CalendarService _calendarService;
        private readonly CalendarDescriptor _calendarDescriptor;

        public SoilWaterService(SunlightService sunlightService,
                                TemperatureService temperatureService,
                                AirHumidityService airHumidityService,
                                TimeService timeService,
                                CalendarService calendarService,
                                CalendarDescriptor calendarDescriptor)
        {
            _sunlightService = sunlightService;
            _temperatureService = temperatureService;
            _airHumidityService = airHumidityService;
            _timeService = timeService;
            _calendarService = calendarService;
            _calendarDescriptor = calendarDescriptor;
        }

        public float CalculatePrecipitations()
        {
            TimeModel today = _timeService.GetToday();
            WeatherModel todayWeather = _calendarService.GetWeatherModel(today.CurrentDay, today.CurrentMonth);
            return todayWeather.WeatherType == WeatherType.Snow ? 0f : todayWeather.Precipitations;
        }

        public float CalculateEvaporation(float soilWater)
        {
            MonthType currentMonth = (MonthType) _timeService.GetToday().CurrentMonth;
            CalendarMonthModel calendarMonthModel = _calendarDescriptor.FindByType(DachaPlaceType.Middle, currentMonth);
            WaterEvaporationSettings waterEvaporationSettings = calendarMonthModel.EvaporationSettings;

            float dailySunAmount = _sunlightService.GetDailySunAmount();
            float dailyAverageTemperature = _temperatureService.GetDailyAverageTemperature();
            float dailyAirHumidity = _airHumidityService.GetDailyAirHumidity();
            float evaporation = CalculateEvaporation(soilWater, dailySunAmount, dailyAverageTemperature, dailyAirHumidity, waterEvaporationSettings);
            return evaporation;
        }

        private float CalculateEvaporation(float soilWater,
                                           float dailySunAmount,
                                           float dailyAverageTemperature,
                                           float dailyAirHumidity,
                                           WaterEvaporationSettings waterEvaporationSettings)
        {
            float solarRadiation = waterEvaporationSettings.SolarRadiation;
            float monthSunHours = waterEvaporationSettings.NormalSunHours;

            float wiltPoint = waterEvaporationSettings.WiltPoint;
            float waterCapacityUnits = waterEvaporationSettings.WaterCapacityUnits;
            float evaporationLimiterValue = waterEvaporationSettings.EvaporationLimiterValue;
            float actualSoilEvaporationMultiplier = waterEvaporationSettings.ActualSoilEvaporationMultiplier;

            float currentSolarRadiation = CalculateDailySolarRadiation(dailySunAmount, monthSunHours, solarRadiation);
            float possibleEvaporation =
                    CalculatePossibleEvaporationWithTurcPotential(dailyAverageTemperature, dailyAirHumidity, currentSolarRadiation);
            float moistureAvailability = ComputeMoistureAvailability(soilWater, wiltPoint, waterCapacityUnits);
            float actualEvaporationLimiter = Mathf.Pow(moistureAvailability, evaporationLimiterValue);
            float actualEvaporationValue = possibleEvaporation * actualEvaporationLimiter;
            float soilWaterDeltaValue = actualEvaporationValue * actualSoilEvaporationMultiplier;

            Debug.Log($"Evaporation test. Current params: dailySun={dailySunAmount}, temp={dailyAverageTemperature}, airHumidity={dailyAirHumidity}. \n"
                      + $"Results: SoilWater={soilWater} \n " + $"PotentialEvaporation={possibleEvaporation} \n "
                      + $"evaporationLimiter01={actualEvaporationLimiter} \n " + $"actualEvaporationMm={actualEvaporationValue} \n "
                      + $"deltaSoilWaterUnits={soilWaterDeltaValue} \n ");

            return soilWaterDeltaValue;
        }

        private float CalculateDailySolarRadiation(float sunHours, float possibleSunHours, float sunRadiation)
        {
            float sunActiveRatio = (possibleSunHours <= 0f) ? 0f : Mathf.Clamp01(sunHours / possibleSunHours);
            return sunRadiation * sunActiveRatio;
        }

        private float CalculatePossibleEvaporationWithTurcPotential(float airTempAvg, float relativeHumidity01, float solarRadiation)
        {
            if (airTempAvg <= 0f) {
                return 0f;
            }

            float humidityDeficit = Mathf.Clamp01(1f - relativeHumidity01);
            float temperatureFactor = airTempAvg / (airTempAvg + TurcTemperatureScaleC);
            float radiationWaterEquivalentMm = RadiationToWaterScale * solarRadiation;
            float humidityFactor = 1f + HumidityDeficitWeight * humidityDeficit;

            return TurcCoefficient * temperatureFactor * radiationWaterEquivalentMm * humidityFactor;
        }

        private float ComputeMoistureAvailability(float soilWater, float wiltPoint, float waterCapacityUnits)
        {
            return waterCapacityUnits <= wiltPoint ? 1f : Mathf.Clamp01((soilWater - wiltPoint) / (waterCapacityUnits - wiltPoint));
        }
    }
}