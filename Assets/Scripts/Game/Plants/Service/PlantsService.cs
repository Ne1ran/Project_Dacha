using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Diseases.Service;
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
        private readonly PlantDiseaseService _plantDiseaseService;
        private readonly IDescriptorService _descriptorService;

        private IDisposable? _disposable;

        public PlantsService(PlantsRepo plantsRepo,
                             IDescriptorService descriptorService,
                             ISubscriber<string, DayChangedEvent> dayChangedSubscriber,
                             SoilService soilService,
                             PlantDiseaseService plantDiseaseService)
        {
            _plantsRepo = plantsRepo;
            _descriptorService = descriptorService;
            _soilService = soilService;
            _plantDiseaseService = plantDiseaseService;

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

        public void InspectPlant(string tileId)
        {
            if (_plantsRepo.Exists(tileId)) {
                Debug.LogWarning($"Plant doesn't exists on tile={tileId}");
                return;
            }

            PlantModel plantModel = _plantsRepo.Require(tileId);
            if (plantModel.InspectedToday) {
                Debug.Log($"No need to double inspect plant on tile={tileId}");
                return;
            }
            
            if (plantModel.DiseaseModels.Count > 0) {
                _plantDiseaseService.CheckSymptoms(plantModel);
            }

            plantModel.InspectedToday = true;
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

            PlantModel plantModel = new(seedId, plantsDescriptorModel.FamilyType, PlantGrowStage.SEED, seedsDescriptorModel.StartHealth,
                                        seedsDescriptorModel.StartImmunity);
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

                SimulatePlantLife(plant, tileId, plantsDescriptorModel, evt.DayDifference);
            }
        }

        private void SimulatePlantLife(PlantModel plant, string soilId, PlantsDescriptorModel plantsDescriptorModel, int dayDifference)
        {
            PlantGrowStage plantCurrentStage = plant.CurrentStage;
            PlantStageDescriptor plantStageDescriptor =
                    plantsDescriptorModel.Stages.Find(stageDescriptor => stageDescriptor.Stage == plantCurrentStage);
            if (plantStageDescriptor == null) {
                throw new KeyNotFoundException($"Stage not found for plant={plant.PlantId}, stage={plantCurrentStage}");
            }

            if (plant.CurrentStage == PlantGrowStage.DEAD) {
                return;
            }

            PlantGrowCalculationModel growCalculationModel = new();

            CalculateConsumptionMultiplier(plantStageDescriptor.PlantConsumption, soilId, growCalculationModel);
            CalculateSunlightAffect(plantStageDescriptor.SunlightParameters, growCalculationModel);
            CalculateTemperatureAffect(plantStageDescriptor.TemperatureParameters, growCalculationModel);
            CalculateAirHumidityAffect(plantStageDescriptor.AirHumidityParameters, growCalculationModel);
            CalculateSoilHumidityAffect(plantStageDescriptor.SoilHumidityParameters, soilId, growCalculationModel);
            Debug.LogWarning($"Plant life simulation. GrowCalcModel: growMultiplier={growCalculationModel.GrowMultiplier}, damage={growCalculationModel.Damage}");

            ApplyGrowCalculationModel(plant, soilId, plantStageDescriptor, growCalculationModel, dayDifference);

            if (plant.CurrentStage == PlantGrowStage.DEAD) {
                // if plant have died - no need for further calculations 
                return;
            }

            TryGrowToNextStage(plant, plantStageDescriptor, plantsDescriptorModel);
            _plantDiseaseService.UpdateDiseases(plant, plantsDescriptorModel, soilId);
        }

        private PlantModel ApplyGrowCalculationModel(PlantModel plant,
                                                     string soilId,
                                                     PlantStageDescriptor plantStageDescriptor,
                                                     PlantGrowCalculationModel calculationModel,
                                                     float dayDifference)
        {
            float growthMultiplier = dayDifference * calculationModel.GrowMultiplier;
            float dayCoeff = growthMultiplier / plantStageDescriptor.AverageGrowTime;

            if (calculationModel.Damage > 0) {
                plant.DealDamage(calculationModel.Damage);
            }

            if (!TryConsumeElements(plant, soilId, plantStageDescriptor, dayCoeff)) {
                // Maybe additional damage?
                return plant;
            }

            plant.StageGrowth += growthMultiplier;
            return plant;
        }

        private PlantModel TryGrowToNextStage(PlantModel plant,
                                              PlantStageDescriptor currentStageDescriptor,
                                              PlantsDescriptorModel plantsDescriptorModel)
        {
            if (plant.StageGrowth < currentStageDescriptor.AverageGrowTime) {
                return plant;
            }

            PlantStageDescriptor? plantStageDescriptor = plantsDescriptorModel.Stages.Find(stageDesc => stageDesc.Stage == plant.CurrentStage);
            if (plantStageDescriptor == null) {
                throw new KeyNotFoundException($"PlantStageDescriptor not found. Stage={plant.CurrentStage.ToString()}");
            }

            int stageIndex = plantsDescriptorModel.Stages.IndexOf(plantStageDescriptor);
            int newStageIndex = stageIndex + 1;
            if (newStageIndex >= plantsDescriptorModel.Stages.Count) {
                Debug.LogWarning("Can't grow to next stage, because stage index is out of range!");
                return plant;
            }

            PlantStageDescriptor newStageDescriptor = plantsDescriptorModel.Stages[newStageIndex];
            plant.CurrentStage = newStageDescriptor.Stage;
            plant.StageGrowth = 0f;
            return plant;
        }

        private bool TryConsumeElements(PlantModel plant, string soilId, PlantStageDescriptor plantStageDescriptor, float dayCoeff)
        {
            float nitrogenUsage = plantStageDescriptor.PlantConsumption.NitrogenUsage * dayCoeff;
            float potassiumUsage = plantStageDescriptor.PlantConsumption.PotassiumUsage * dayCoeff;
            float phosphorusUsage = plantStageDescriptor.PlantConsumption.PhosphorusUsage * dayCoeff;
            float waterUsage = plantStageDescriptor.PlantConsumption.WaterUsage * dayCoeff;
            ElementsModel elementsModel = new(nitrogenUsage, potassiumUsage, phosphorusUsage);
            bool result = _soilService.TryConsumeForPlant(soilId, waterUsage, elementsModel);
            if (result) {
                plant.TakenElements.Add(elementsModel);
            }

            return result;
        }

        private PlantGrowCalculationModel CalculateConsumptionMultiplier(PlantConsumptionDescriptor consumptionDescriptor,
                                                                         string soilId,
                                                                         PlantGrowCalculationModel calculationModel)
        {
            SoilModel soilModel = _soilService.RequireSoil(soilId);
            if (consumptionDescriptor.WaterUsage > soilModel.WaterAmount) {
                calculationModel.GrowMultiplier += consumptionDescriptor.GrowDebuff;
            }

            calculationModel.GrowMultiplier +=
                    consumptionDescriptor.NitrogenUsage * consumptionDescriptor.PreferredUsageMultiplier < soilModel.Elements.Nitrogen
                            ? consumptionDescriptor.GrowBuff
                            : consumptionDescriptor.GrowDebuff;
            calculationModel.GrowMultiplier +=
                    consumptionDescriptor.PotassiumUsage * consumptionDescriptor.PreferredUsageMultiplier < soilModel.Elements.Potassium
                            ? consumptionDescriptor.GrowBuff
                            : consumptionDescriptor.GrowDebuff;
            calculationModel.GrowMultiplier +=
                    consumptionDescriptor.PhosphorusUsage * consumptionDescriptor.PreferredUsageMultiplier < soilModel.Elements.Phosphorus
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
                                                                      string soilId,
                                                                      PlantGrowCalculationModel calculationModel)
        {
            if (soilHumidityParameters.Ignore) {
                return calculationModel;
            }

            float soilHumidity = _soilService.GetSoilHumidity(soilId);

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