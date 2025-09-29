using System;
using Core.Attributes;
using Game.Calendar.Event;
using Game.GameMap.Tiles.Event;
using Game.Humidity.Service;
using Game.Sunlight.Service;
using Game.Temperature.Service;
using MessagePipe;
using UnityEngine;

namespace Game.Soil.Service
{
    [Service]
    public class WaterEvaporationService : IDisposable
    {
        private readonly SunlightService _sunlightService;
        private readonly TemperatureService _temperatureService;
        private readonly AirHumidityService _airHumidityService;

        private IDisposable? _disposable;

        private const float TurcTemperatureScaleC = 15f; // Turc formula: (T / (T + 15))
        private const float TurcCoefficient = 0.013f; // Turc coeff
        private const float RadiationToWaterScale = 23.9f; // Equivalent water in Turc's formulas
        private const float HumidityDeficitWeight = 0.4f; // Weight of water deficit (1 - RH)

        // Погодно-астрономические параметры
        public float referenceDaylengthHours = 12f; // N_ref — максимум «солнечных часов» для нормировки
        public float clearSkyRadiation = 25f; // Rs_clear — радиация ясного дня

        // Почва и калибровка
        public float soilWaterAtWiltingPointUnits = 10f; // W_wp — «точка завядания» в игровых единицах
        public float soilWaterAtFieldCapacityUnits = 100f; // W_fc — «полевая влагоёмкость» в игровых единицах
        public float evaporationLimiterCurvePower = 0.7f; // α — кривизна ограничителя по влаге
        public float millimeterToSoilUnits = 7.5f; // сколько игровых единиц воды соответствует 1 мм испарения

        public WaterEvaporationService(SunlightService sunlightService,
                                       TemperatureService temperatureService,
                                       AirHumidityService airHumidityService,
                                       IPublisher<string, SoilUpdatedEvent> soilUpdatedPublisher,
                                       ISubscriber<string, DayChangedEvent> dayFinishedSubscriber)
        {
            _sunlightService = sunlightService;
            _temperatureService = temperatureService;
            _airHumidityService = airHumidityService;
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

        public void TestEvaporation(float soilWater)
        {
            float dailySunAmount = _sunlightService.GetDailySunAmount();
            float dailyAverageTemperature = _temperatureService.GetDailyAverageTemperature();
            float dailyAirHumidity = _airHumidityService.GetDailyAirHumidity();
            (float potentialEvapotranspirationMm, float evaporationLimiter01, float actualEvaporationMm, float deltaSoilWaterUnits) valueTuple =
                    Step(ref soilWater, dailySunAmount, dailyAverageTemperature, dailyAirHumidity);
            
            Debug.LogWarning($"Evaporation test. Current params: dailySun={dailySunAmount}, temp={dailyAverageTemperature}, airHumidity={dailyAirHumidity}. \n"
                             + $"Results: SoilWater={soilWater} \n "
                             + $"PotentialEvaporation={valueTuple.potentialEvapotranspirationMm} \n "
                             + $"evaporationLimiter01={valueTuple.evaporationLimiter01} \n "
                             + $"actualEvaporationMm={valueTuple.actualEvaporationMm} \n "
                             + $"deltaSoilWaterUnits={valueTuple.deltaSoilWaterUnits} \n ");
        }

        private (float potentialEvapotranspirationMm, float evaporationLimiter01, float actualEvaporationMm, float deltaSoilWaterUnits) Step(
                ref float soilWaterUnits,
                float dailySunAmount,
                float dailyAverageTemperature,
                float dailyAirHumidity)
        {
            float solarRadiation = EstimateDailySolarRadiation(dailySunAmount, referenceDaylengthHours, clearSkyRadiation);
            float potentialEvapotranspirationMm = ComputeTurcPotential(dailyAverageTemperature, dailyAirHumidity, solarRadiation);
            float moistureAvailability01 = ComputeMoistureAvailability(soilWaterUnits, soilWaterAtWiltingPointUnits, soilWaterAtFieldCapacityUnits);
            float evaporationLimiter01 = Mathf.Pow(moistureAvailability01, evaporationLimiterCurvePower);
            float actualEvaporationMm = potentialEvapotranspirationMm * evaporationLimiter01;
            float deltaSoilWaterUnits = actualEvaporationMm * millimeterToSoilUnits;
            soilWaterUnits = Mathf.Max(1f, soilWaterUnits - deltaSoilWaterUnits);
            return (potentialEvapotranspirationMm, evaporationLimiter01, actualEvaporationMm, deltaSoilWaterUnits);
        }

        private float EstimateDailySolarRadiation(float sunshineHours, float referenceDaylengthHours, float clearSkyRadiation)
        {
            float sunshineRatio01 = (referenceDaylengthHours <= 0f) ? 0f : Mathf.Clamp01(sunshineHours / referenceDaylengthHours);
            return clearSkyRadiation * sunshineRatio01;
        }

        private float ComputeTurcPotential(float airTempAvg, float relativeHumidity01, float solarRadiation)
        {
            if (airTempAvg <= 0f) {
                return 0f;
            }
            float humidityDeficit01 = Mathf.Clamp01(1f - relativeHumidity01);
            float temperatureFactor = airTempAvg / (airTempAvg + TurcTemperatureScaleC);
            float radiationWaterEquivalentMm = RadiationToWaterScale * solarRadiation;
            float humidityFactor = 1f + HumidityDeficitWeight * humidityDeficit01;

            return TurcCoefficient * temperatureFactor * radiationWaterEquivalentMm * humidityFactor;
        }

        private float ComputeMoistureAvailability(float soilWaterUnits, float soilWaterAtWiltingPointUnits, float soilWaterAtFieldCapacityUnits)
        {
            return soilWaterAtFieldCapacityUnits <= soilWaterAtWiltingPointUnits
                           ? 1f
                           : Mathf.Clamp01((soilWaterUnits - soilWaterAtWiltingPointUnits)
                                           / (soilWaterAtFieldCapacityUnits - soilWaterAtWiltingPointUnits));
        }
    }
}