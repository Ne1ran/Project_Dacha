using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Plants.Repo;
using Game.Seeds.Descriptors;
using Game.TimeMove.Event;
using MessagePipe;
using UnityEngine;

namespace Game.Plants.Service
{
    [Service]
    public class PlantsService : IDisposable
    {
        private readonly PlantsRepo _plantsRepo;
        private readonly SoilService _soilService;
        private readonly IDescriptorService _descriptorService;

        private IDisposable? _disposable;

        public PlantsService(PlantsRepo plantsRepo,
                             IDescriptorService descriptorService,
                             ISubscriber<string, DayChangedEvent> dayChangedSubscriber,
                             SoilService soilService)
        {
            _plantsRepo = plantsRepo;
            _descriptorService = descriptorService;
            _soilService = soilService;

            DisposableBagBuilder bag = DisposableBag.CreateBuilder();

            bag.Add(dayChangedSubscriber.Subscribe(DayChangedEvent.DAY_FINISHED, OnDayFinished));

            _disposable = bag.Build();
        }

        public void InitializeFromSave()
        {
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public void CreatePlant(string seedId, string tileId)
        {
            if (_plantsRepo.Exists(tileId)) {
                Debug.LogWarning($"Plant already exists seedId={seedId}");
                return;
            }

            PlantsDescriptor plantsDescriptor = _descriptorService.Require<PlantsDescriptor>();
            PlantsDescriptorModel? plantsDescriptorModel = plantsDescriptor.Items.Find(plant => plant.PlantId == seedId);
            if (plantsDescriptorModel == null) {
                Debug.LogWarning($"Plant not found seedId={seedId}");
                return;
            }

            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();
            SeedsDescriptorModel seedsDescriptorModel = seedsDescriptor.Items.Find(seed => seed.SeedId == seedId);
            if (seedsDescriptorModel == null) {
                Debug.LogWarning($"Seed descriptor not found seedId={seedId}");
                return;
            }

            PlantModel plantModel = new(seedId, PlantGrowStage.SEED, seedsDescriptorModel.StartHealth, seedsDescriptorModel.StartImmunity);
            _plantsRepo.Save(tileId, plantModel);
        }

        private void OnDayFinished(DayChangedEvent evt)
        {
            Dictionary<string, PlantModel> plants = _plantsRepo.GetAll();

            PlantsDescriptor plantsDescriptor = _descriptorService.Require<PlantsDescriptor>();
            foreach ((string tileId, PlantModel plant) in plants) {
                PlantsDescriptorModel? plantsDescriptorModel = plantsDescriptor.Items.Find(plantModel => plant.PlantId == plantModel.PlantId);
                if (plantsDescriptorModel == null) {
                    continue;
                }

                SoilModel soilModel = _soilService.RequireSoil(tileId);
                SimulatePlantLife(plant, soilModel, plantsDescriptorModel, evt.DayDifference);
            }
        }

        private void SimulatePlantLife(PlantModel plant, SoilModel soil, PlantsDescriptorModel plantsDescriptorModel, int dayDifference)
        {
            PlantGrowStage plantCurrentStage = plant.CurrentStage;
            PlantStageDescriptor plantStageDescriptor =
                    plantsDescriptorModel.Stages.Find(stageDescriptor => stageDescriptor.Stage == plantCurrentStage);
            if (plantStageDescriptor == null) {
                throw new KeyNotFoundException($"Stage not found for plant={plant.PlantId}, stage={plantCurrentStage}");
            }

            PlantGrowCalculationModel growCalculationModel = new();

            CalculateConsumptionMultiplier(plantStageDescriptor.PlantConsumption, soil, growCalculationModel);
            CalculateSunlightAffect(plantStageDescriptor.SunlightParameters, growCalculationModel);
            CalculateTemperatureAffect(plantStageDescriptor.TemperatureParameters, growCalculationModel);
            CalculateAirHumidityAffect(plantStageDescriptor.AirHumidityParameters, growCalculationModel);
            CalculateSoilHumidityAffect(plantStageDescriptor.SoilHumidityParameters, soil, growCalculationModel);

            Debug.LogWarning($"Plant life simulation. GrowCalcModel: growMultiplier={growCalculationModel.GrowMultiplier}, damage={growCalculationModel.Damage}");
        }

        private PlantGrowCalculationModel CalculateConsumptionMultiplier(PlantConsumptionDescriptor consumptionDescriptor,
                                                                         SoilModel soilModel,
                                                                         PlantGrowCalculationModel calculationModel)
        {
            if (consumptionDescriptor.WaterUsage > soilModel.WaterAmount) {
                calculationModel.GrowMultiplier += consumptionDescriptor.GrowDebuff;
            }

            calculationModel.GrowMultiplier += consumptionDescriptor.NitrogenUsage < soilModel.Elements.Nitrogen
                                                       ? consumptionDescriptor.GrowBuff
                                                       : consumptionDescriptor.GrowDebuff;
            calculationModel.GrowMultiplier += consumptionDescriptor.PotassiumUsage < soilModel.Elements.Potassium
                                                       ? consumptionDescriptor.GrowBuff
                                                       : consumptionDescriptor.GrowDebuff;
            calculationModel.GrowMultiplier += consumptionDescriptor.PhosphorusUsage < soilModel.Elements.Phosphorus
                                                       ? consumptionDescriptor.GrowBuff
                                                       : consumptionDescriptor.GrowDebuff;

            return calculationModel;
        }

        private PlantGrowCalculationModel CalculateSunlightAffect(PlantSunlightParameters sunlightParameters,
                                                                  PlantGrowCalculationModel calculationModel)
        {
            if (sunlightParameters.Ignore) {
                return calculationModel;
            }

            float currentSunlight = 2200f; // todo neiran integrate sunlight and weather system

            if (sunlightParameters.MinSunlight > currentSunlight) {
                calculationModel.Damage += sunlightParameters.DamagePerDeviation * Mathf.Abs(currentSunlight - sunlightParameters.MinSunlight);
            }

            if (sunlightParameters.MaxSunlight < currentSunlight) {
                calculationModel.Damage += sunlightParameters.DamagePerDeviation * Mathf.Abs(currentSunlight - sunlightParameters.MaxSunlight);
            }

            if (sunlightParameters.MinPreferredSunlight < currentSunlight && sunlightParameters.MaxPreferredSunlight > currentSunlight) {
                calculationModel.GrowMultiplier += sunlightParameters.GrowBuff;
            }

            return calculationModel;
        }

        private PlantGrowCalculationModel CalculateTemperatureAffect(PlantTemperatureParameters temperatureParameters,
                                                                     PlantGrowCalculationModel calculationModel)
        {
            if (temperatureParameters.Ignore) {
                return calculationModel;
            }

            float currentTemperature = 25f; // todo neiran integrate temperature and add it to soil system

            if (temperatureParameters.MinTemperature > currentTemperature) {
                calculationModel.Damage += temperatureParameters.DamagePerDeviation
                                           * Mathf.Abs(currentTemperature - temperatureParameters.MinTemperature);
            }

            if (temperatureParameters.MaxTemperature < currentTemperature) {
                calculationModel.Damage += temperatureParameters.DamagePerDeviation
                                           * Mathf.Abs(currentTemperature - temperatureParameters.MaxTemperature);
            }

            if (temperatureParameters.MinPreferredTemperature < currentTemperature
                && temperatureParameters.MaxPreferredTemperature > currentTemperature) {
                calculationModel.GrowMultiplier += temperatureParameters.GrowBuff;
            }

            return calculationModel;
        }

        private PlantGrowCalculationModel CalculateAirHumidityAffect(PlantHumidityParameters airHumidityParameters,
                                                                     PlantGrowCalculationModel calculationModel)
        {
            if (airHumidityParameters.Ignore) {
                return calculationModel;
            }

            float airHumidityPercent = 70f; // todo neiran integrate sunlight and weather system

            if (airHumidityParameters.MinHumidity > airHumidityPercent) {
                calculationModel.Damage +=
                        airHumidityParameters.DamagePerDeviation * Mathf.Abs(airHumidityPercent - airHumidityParameters.MinHumidity);
            }

            if (airHumidityParameters.MaxHumidity < airHumidityPercent) {
                calculationModel.Damage +=
                        airHumidityParameters.DamagePerDeviation * Mathf.Abs(airHumidityPercent - airHumidityParameters.MaxHumidity);
            }

            if (airHumidityParameters.MinPreferredHumidity < airHumidityPercent && airHumidityParameters.MaxPreferredHumidity > airHumidityPercent) {
                calculationModel.GrowMultiplier += airHumidityParameters.GrowBuff;
            }

            return calculationModel;
        }

        private PlantGrowCalculationModel CalculateSoilHumidityAffect(PlantHumidityParameters soilHumidityParameters,
                                                                      SoilModel soilModel,
                                                                      PlantGrowCalculationModel calculationModel)
        {
            if (soilHumidityParameters.Ignore) {
                return calculationModel;
            }

            float soilHumidity = soilModel.WaterAmount * soilModel.Breathability / 100f;

            if (soilHumidityParameters.MinHumidity > soilHumidity) {
                calculationModel.Damage += soilHumidityParameters.DamagePerDeviation * Mathf.Abs(soilHumidity - soilHumidityParameters.MinHumidity);
            }

            if (soilHumidityParameters.MaxHumidity < soilHumidity) {
                calculationModel.Damage += soilHumidityParameters.DamagePerDeviation * Mathf.Abs(soilHumidity - soilHumidityParameters.MaxHumidity);
            }

            if (soilHumidityParameters.MinPreferredHumidity < soilHumidity && soilHumidityParameters.MaxPreferredHumidity > soilHumidity) {
                calculationModel.GrowMultiplier += soilHumidityParameters.GrowBuff;
            }

            return calculationModel;
        }
    }
}