using System;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Descriptor;
using Game.Calendar.Event;
using Game.Calendar.Model;
using Game.Calendar.Service;
using Game.Difficulty.Model;
using Game.Evaporation.Descriptor;
using Game.GameMap.Tiles.Event;
using Game.Humidity.Service;
using Game.Sunlight.Service;
using Game.Temperature.Service;
using MessagePipe;
using UnityEngine;

namespace Game.Evaporation.Service
{
    [Service]
    public class WaterEvaporationService : IDisposable
    {
        private const float TurcTemperatureScaleC = 15f; // Turc formula: (T / (T + 15))
        private const float TurcCoefficient = 0.013f; // Turc coeff
        private const float RadiationToWaterScale = 23.9f; // Equivalent water in Turc's formulas
        private const float HumidityDeficitWeight = 0.4f; // Weight of water deficit (1 - RH)

        private readonly SunlightService _sunlightService;
        private readonly TemperatureService _temperatureService;
        private readonly AirHumidityService _airHumidityService;
        private readonly TimeService _timeService;
        private readonly IDescriptorService _descriptorService;

        private IDisposable? _disposable;

        public WaterEvaporationService(SunlightService sunlightService,
                                       TemperatureService temperatureService,
                                       AirHumidityService airHumidityService,
                                       IDescriptorService descriptorService,
                                       TimeService timeService,
                                       IPublisher<string, SoilUpdatedEvent> soilUpdatedPublisher,
                                       ISubscriber<string, DayChangedEvent> dayFinishedSubscriber)
        {
            _sunlightService = sunlightService;
            _temperatureService = temperatureService;
            _airHumidityService = airHumidityService;
            _descriptorService = descriptorService;
            _timeService = timeService;
            DisposableBagBuilder bagBuilder = DisposableBag.CreateBuilder();
            bagBuilder.Add(dayFinishedSubscriber.Subscribe(DayChangedEvent.DAY_FINISHED, OnDayFinished));
            _disposable = bagBuilder.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        private void OnDayFinished(DayChangedEvent dayChangedEvent)
        {
            // todo neiran integrate properly
        }

        public float TestEvaporation(float soilWater)
        {
            CalendarDescriptor calendarDescriptor = _descriptorService.Require<CalendarDescriptor>();
            MonthType currentMonth = (MonthType) _timeService.GetToday().CurrentMonth;
            CalendarMonthModel calendarMonthModel = calendarDescriptor.FindByType(DachaPlaceType.Middle, currentMonth);
            WaterEvaporationSettings waterEvaporationSettings = calendarMonthModel.EvaporationSettings;

            float dailySunAmount = _sunlightService.GetDailySunAmount();
            float dailyAverageTemperature = _temperatureService.GetDailyAverageTemperature();
            float dailyAirHumidity = _airHumidityService.GetDailyAirHumidity();
            float evaporation = CalculateEvaporation(ref soilWater, dailySunAmount, dailyAverageTemperature, dailyAirHumidity,
                                                     waterEvaporationSettings);
            return evaporation;
        }

        private float CalculateEvaporation(ref float soilWaterUnits,
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
            float moistureAvailability = ComputeMoistureAvailability(soilWaterUnits, wiltPoint, waterCapacityUnits);
            float actualEvaporationLimiter = Mathf.Pow(moistureAvailability, evaporationLimiterValue);
            float actualEvaporationValue = possibleEvaporation * actualEvaporationLimiter;
            float soilWaterDeltaValue = actualEvaporationValue * actualSoilEvaporationMultiplier;

            Debug.Log($"Evaporation test. Current params: dailySun={dailySunAmount}, temp={dailyAverageTemperature}, airHumidity={dailyAirHumidity}. \n"
                      + $"Results: SoilWater={soilWaterUnits} \n " + $"PotentialEvaporation={possibleEvaporation} \n "
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

        private float ComputeMoistureAvailability(float soilWaterUnits, float wiltPoint, float waterCapacityUnits)
        {
            return waterCapacityUnits <= wiltPoint ? 1f : Mathf.Clamp01((soilWaterUnits - wiltPoint) / (waterCapacityUnits - wiltPoint));
        }
    }
}