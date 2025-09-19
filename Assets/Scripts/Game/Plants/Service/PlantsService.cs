using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Diseases.Model;
using Game.Diseases.Service;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using Game.Plants.Descriptors;
using Game.Plants.Event;
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
        private readonly IPublisher<string, PlantUpdatedEvent> _plantUpdatedEvent;

        private IDisposable? _disposable;

        public PlantsService(PlantsRepo plantsRepo,
                             IDescriptorService descriptorService,
                             ISubscriber<string, DayChangedEvent> dayChangedSubscriber,
                             SoilService soilService,
                             PlantDiseaseService plantDiseaseService,
                             IPublisher<string, PlantUpdatedEvent> plantUpdatedEvent)
        {
            _plantsRepo = plantsRepo;
            _descriptorService = descriptorService;
            _soilService = soilService;
            _plantDiseaseService = plantDiseaseService;
            _plantUpdatedEvent = plantUpdatedEvent;

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

        public PlantModel? GetPlant(string tileId)
        {
            return _plantsRepo.Get(tileId);
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

        public void RemovePlant(string tileId)
        {
            PlantModel plantModel = _plantsRepo.Require(tileId);
            List<SavedDiseaseModel> savedDiseaseModels = _plantDiseaseService.GetSavedDiseases(plantModel);
            _soilService.InfectSoil(tileId, savedDiseaseModels);
            _plantsRepo.Delete(tileId);
            _plantUpdatedEvent.Publish(PlantUpdatedEvent.Removed, new(tileId, plantModel));
        }

        public void CreatePlant(string seedId, string tileId)
        {
            if (_plantsRepo.Exists(tileId)) {
                Debug.LogWarning($"Plant already exists seedId={seedId}");
                return;
            }

            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();
            SeedsDescriptorModel seedsDescriptorModel = seedsDescriptor.Items.Find(seed => seed.Id == seedId);
            if (seedsDescriptorModel == null) {
                Debug.LogWarning($"Seed descriptor not found seedId={seedId}");
                return;
            }

            string plantId = seedsDescriptorModel.PlantId;
            PlantsDescriptor plantsDescriptor = _descriptorService.Require<PlantsDescriptor>();
            PlantsDescriptorModel? plantsDescriptorModel = plantsDescriptor.Items.Find(plant => plant.Id == plantId);
            if (plantsDescriptorModel == null) {
                Debug.LogWarning($"Plant not found seedId={seedId}, plantId={plantId}");
                return;
            }

            PlantModel plantModel = new(plantId, plantsDescriptorModel.FamilyType, PlantGrowStage.SEED, seedsDescriptorModel.StartHealth,
                                        seedsDescriptorModel.StartImmunity);
            _plantUpdatedEvent.Publish(PlantUpdatedEvent.Created, new(tileId, plantModel));
            _plantsRepo.Save(tileId, plantModel);
        }

        private void OnDayFinished(DayChangedEvent evt)
        {
            Dictionary<string, PlantModel> plants = _plantsRepo.GetAll();

            PlantsDescriptor plantsDescriptor = _descriptorService.Require<PlantsDescriptor>();
            foreach ((string tileId, PlantModel plant) in plants) {
                PlantsDescriptorModel? plantsDescriptorModel = plantsDescriptor.Items.Find(plantModel => plant.PlantId == plantModel.Id);
                if (plantsDescriptorModel == null) {
                    continue;
                }

                SimulatePlantLife(plant, tileId, plantsDescriptorModel, evt.DayDifference);
                _plantUpdatedEvent.Publish(PlantUpdatedEvent.Updated, new(tileId, plant));
            }
        }

        private void SimulatePlantLife(PlantModel plant, string soilId, PlantsDescriptorModel plantsDescriptorModel, int dayDifference)
        {
            PlantGrowStage plantCurrentStage = plant.CurrentStage;
            if (plant.CurrentStage == PlantGrowStage.DEAD) {
                return;
            }

            PlantStageDescriptor plantStageDescriptor =
                    plantsDescriptorModel.Stages.Find(stageDescriptor => stageDescriptor.Stage == plantCurrentStage);
            if (plantStageDescriptor == null) {
                throw new KeyNotFoundException($"Stage not found for plant={plant.PlantId}, stage={plantCurrentStage}");
            }

            PlantGrowCalculationModel growCalculationModel = new();

            CalculateConsumptionMultiplier(plantStageDescriptor.PlantConsumption, soilId, ref growCalculationModel);
            CalculateSunlightAffect(plantStageDescriptor.SunlightParameters, ref growCalculationModel);
            CalculateTemperatureAffect(plantStageDescriptor.TemperatureParameters, ref growCalculationModel);
            CalculateAirHumidityAffect(plantStageDescriptor.AirHumidityParameters, ref growCalculationModel);
            CalculateSoilHumidityAffect(plantStageDescriptor.SoilHumidityParameters, soilId, ref growCalculationModel);
            Debug.LogWarning($"Plant life simulation. GrowCalcModel: growMultiplier={growCalculationModel.GrowMultiplier}, damage={growCalculationModel.Damage}");

            if (!TryApplyGrowCalculationModel(ref plant, soilId, plantStageDescriptor, growCalculationModel, dayDifference)) {
                _plantDiseaseService.UpdatePlantDiseases(ref plant, plantsDescriptorModel, soilId);
                return;
            }

            if (plant.CurrentStage == PlantGrowStage.DEAD) {
                // if plant have died - no need for further calculations 
                return;
            }

            TryGrowToNextStage(plant, plantStageDescriptor, plantsDescriptorModel);
            if (growCalculationModel.Damage <= 0) {
                TryHealPlant(ref plant, plantStageDescriptor, soilId);
                if (growCalculationModel.GrowMultiplier > 1f) {
                    TryIncreaseImmunity(ref plant, plantStageDescriptor);
                }
            }

            _plantDiseaseService.UpdatePlantDiseases(ref plant, plantsDescriptorModel, soilId);
        }

        private void TryHealPlant(ref PlantModel plant, PlantStageDescriptor plantStageDescriptor, string soilId)
        {
            if (plant.Health >= Constants.Constants.MAX_HEALTH) {
                return;
            }

            float neededHealth = Mathf.Min(plantStageDescriptor.DailyRegeneration, Constants.Constants.MAX_HEALTH - plant.Health);
            if (_soilService.TryConsumeHumus(soilId, neededHealth)) {
                plant.Health = Mathf.Clamp(plant.Health + neededHealth, 0f, Constants.Constants.MAX_HEALTH);
            }
        }

        private void TryIncreaseImmunity(ref PlantModel plant, PlantStageDescriptor plantStageDescriptor)
        {
            if (plant.Immunity >= Constants.Constants.MAX_IMMUNITY) {
                return;
            }

            float healthMultiplier = plant.Health / Constants.Constants.MAX_HEALTH;
            plant.Immunity = Mathf.Clamp(plant.Immunity + healthMultiplier * plantStageDescriptor.DailyImmunityGain, 0f,
                                         Constants.Constants.MAX_IMMUNITY);
        }

        private bool TryApplyGrowCalculationModel(ref PlantModel plant,
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
                return false;
            }

            plant.StageGrowth += growthMultiplier;
            Debug.Log($"Plant has grew up! PlantId={plant.PlantId}, stageGrowth={plant.StageGrowth}");
            return true;
        }

        private void TryGrowToNextStage(PlantModel plant, PlantStageDescriptor currentStageDescriptor, PlantsDescriptorModel plantsDescriptorModel)
        {
            if (plant.StageGrowth < currentStageDescriptor.AverageGrowTime) {
                return;
            }

            PlantStageDescriptor? plantStageDescriptor = plantsDescriptorModel.Stages.Find(stageDesc => stageDesc.Stage == plant.CurrentStage);
            if (plantStageDescriptor == null) {
                throw new KeyNotFoundException($"PlantStageDescriptor not found. Stage={plant.CurrentStage.ToString()}");
            }

            int stageIndex = plantsDescriptorModel.Stages.IndexOf(plantStageDescriptor);
            int newStageIndex = stageIndex + 1;
            if (newStageIndex >= plantsDescriptorModel.Stages.Count) {
                return;
            }

            PlantStageDescriptor newStageDescriptor = plantsDescriptorModel.Stages[newStageIndex];
            plant.CurrentStage = newStageDescriptor.Stage;
            plant.StageGrowth = 0f;
            Debug.Log($"Plant has grew to next stage! PlantId={plant.PlantId}, newStage={newStageDescriptor.Stage}");
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

            Debug.Log($"Consume elements. nitrogenUsage={nitrogenUsage}, potassiumUsage={potassiumUsage}, phosphorusUsage={phosphorusUsage}, waterUsage={waterUsage} ");
            return result;
        }

        private void CalculateConsumptionMultiplier(PlantConsumptionDescriptor consumptionDescriptor,
                                                    string soilId,
                                                    ref PlantGrowCalculationModel calculationModel)
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
        }

        private void CalculateSunlightAffect(PlantSunlightParameters sunlightParameters, ref PlantGrowCalculationModel calculationModel)
        {
            if (sunlightParameters.Ignore) {
                return;
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
        }

        private void CalculateTemperatureAffect(PlantTemperatureParameters temperatureParameters, ref PlantGrowCalculationModel calculationModel)
        {
            if (temperatureParameters.Ignore) {
                return;
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
        }

        private void CalculateAirHumidityAffect(PlantHumidityParameters airHumidityParameters, ref PlantGrowCalculationModel calculationModel)
        {
            if (airHumidityParameters.Ignore) {
                return;
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
        }

        private void CalculateSoilHumidityAffect(PlantHumidityParameters soilHumidityParameters,
                                                 string soilId,
                                                 ref PlantGrowCalculationModel calculationModel)
        {
            if (soilHumidityParameters.Ignore) {
                return;
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
        }
    }
}