using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Event;
using Game.Diseases.Model;
using Game.Diseases.Service;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using Game.Humidity.Service;
using Game.Plants.Descriptors;
using Game.Plants.Event;
using Game.Plants.Model;
using Game.Plants.Repo;
using Game.Seeds.Descriptors;
using Game.Stress.Model;
using Game.Sunlight.Service;
using Game.Temperature.Model;
using Game.Temperature.Service;
using Game.Utils;
using MessagePipe;
using UnityEngine;

namespace Game.Plants.Service
{
    [Service]
    public class PlantsService : IDisposable
    {
        private readonly PlantsRepo _plantsRepo;
        private readonly SoilService _soilService;
        private readonly SunlightService _sunlightService;
        private readonly AirHumidityService _airHumidityService;
        private readonly TemperatureService _temperatureService;
        private readonly PlantDiseaseService _plantDiseaseService;
        private readonly IDescriptorService _descriptorService;
        private readonly IPublisher<string, PlantUpdatedEvent> _plantUpdatedEvent;

        private IDisposable? _disposable;

        public PlantsService(PlantsRepo plantsRepo,
                             IDescriptorService descriptorService,
                             ISubscriber<string, DayChangedEvent> dayChangedSubscriber,
                             SoilService soilService,
                             PlantDiseaseService plantDiseaseService,
                             IPublisher<string, PlantUpdatedEvent> plantUpdatedEvent,
                             SunlightService sunlightService,
                             AirHumidityService airHumidityService,
                             TemperatureService temperatureService)
        {
            _plantsRepo = plantsRepo;
            _descriptorService = descriptorService;
            _soilService = soilService;
            _plantDiseaseService = plantDiseaseService;
            _plantUpdatedEvent = plantUpdatedEvent;
            _sunlightService = sunlightService;
            _airHumidityService = airHumidityService;
            _temperatureService = temperatureService;

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

            PlantStageDescriptor plantStageDescriptor = plantsDescriptorModel.RequireStage(plantCurrentStage);
            PlantGrowCalculationModel growModel = GetGrowModel(plant, soilId, plantsDescriptorModel, plantStageDescriptor);
            Debug.LogWarning($"Plant life simulation. GrowCalcModel: growMultiplier={growModel.GrowMultiplier}, damage={growModel.Damage}");

            if (!TryApplyGrowCalculationModel(ref plant, soilId, plantStageDescriptor, plantStageDescriptor.PlantConsumption, growModel,
                                              dayDifference)) {
                _plantDiseaseService.UpdatePlantDiseases(ref plant, plantsDescriptorModel, soilId);
                return;
            }

            if (plant.CurrentStage == PlantGrowStage.DEAD) {
                // if plant have died - no need for further calculations 
                return;
            }

            if (!growModel.BlockHealing) {
                TryHealPlant(ref plant, plantStageDescriptor, soilId);
            }

            if (!growModel.BlockImmunityGain) {
                TryIncreaseImmunity(ref plant, plantStageDescriptor);
            }

            TryGrowToNextStage(plant, plantStageDescriptor, plantsDescriptorModel);
            _plantDiseaseService.UpdatePlantDiseases(ref plant, plantsDescriptorModel, soilId);
        }

        private PlantGrowCalculationModel GetGrowModel(PlantModel plant,
                                                       string soilId,
                                                       PlantsDescriptorModel plantsDescriptorModel,
                                                       PlantStageDescriptor plantStageDescriptor)
        {
            PlantGrowCalculationModel growModel = new();
            SoilModel soilModel = _soilService.RequireSoil(soilId);
            CalculatePhAffect(plantsDescriptorModel.PhParameters, soilModel, ref growModel);

            if (plantStageDescriptor.IncludeSunlight) {
                CalculateSunlightAffect(plantStageDescriptor.SunlightParameters, ref growModel);
            }

            if (plantStageDescriptor.IncludeTemperature) {
                CalculateTemperatureAffect(plantStageDescriptor.TemperatureParameters, ref growModel);
            }

            if (plantStageDescriptor.IncludeAirHumidity) {
                CalculateAirHumidityAffect(plantStageDescriptor.AirHumidityParameters, ref growModel);
            }

            if (plantStageDescriptor.IncludeSoilHumidity) {
                CalculateSoilHumidityAffect(plantStageDescriptor.SoilHumidityParameters, soilModel, ref growModel);
            }

            if (plantStageDescriptor.IncludeSalinity) {
                CalculateSalinityAffect(plantStageDescriptor.SalinityParameters, soilModel, ref growModel);
            }

            if (plantStageDescriptor.IncludeConsumption) {
                CalculateConsumption(plantStageDescriptor.PlantConsumption, soilModel, ref growModel);
            }

            ApplyStress(plant, plantsDescriptorModel.StressParameters, ref growModel);

            return growModel;
        }

        private void ApplyStress(PlantModel plant, PlantStressParameters stressParameters, ref PlantGrowCalculationModel growModel)
        {
            foreach ((StressType stressType, float stressAmount) in growModel.Stress) {
                plant.AddStress(stressType, stressAmount);
            }
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
                                                  PlantConsumptionDescriptor plantConsumptionDescriptor,
                                                  PlantGrowCalculationModel calculationModel,
                                                  float dayDifference)
        {
            float growth = dayDifference * calculationModel.GrowMultiplier;
            float dayCoeff = dayDifference / plantStageDescriptor.AverageGrowTime;

            if (calculationModel.Damage > 0) {
                plant.DealDamage(calculationModel.Damage);
            }

            if (!TryConsumeElements(plant, soilId, plantStageDescriptor, dayCoeff)) {
                calculationModel.Stress.TryAdd(StressType.ConsumptionOverall, plantConsumptionDescriptor.NotEnoughStressGain);
                return false;
            }

            plant.StageGrowth += growth;
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

        private void CalculateConsumption(PlantConsumptionDescriptor consumptionDescriptor,
                                          SoilModel soilModel,
                                          ref PlantGrowCalculationModel growModel)
        {
            float nitrogenUsage = consumptionDescriptor.NitrogenUsage * growModel.GrowMultiplier;
            float potassiumUsage = consumptionDescriptor.PotassiumUsage * growModel.GrowMultiplier;
            float phosphorusUsage = consumptionDescriptor.PhosphorusUsage * growModel.GrowMultiplier;
            float waterUsage = consumptionDescriptor.WaterUsage * growModel.GrowMultiplier;

            SoilConsumptionModel soilConsumptionModel =
                    CreateSoilConsumptionModel(soilModel, nitrogenUsage, potassiumUsage, phosphorusUsage, waterUsage);

            if (!soilConsumptionModel.HasEnoughWater) {
                growModel.Stress.AddOrSum(StressType.ConsumptionWater, consumptionDescriptor.StressGainWater);
            }

            if (!soilConsumptionModel.HasEnoughNitrogen) {
                growModel.Stress.AddOrSum(StressType.ConsumptionNitrogen, consumptionDescriptor.StressGainNitrogen);
            }

            if (!soilConsumptionModel.HasEnoughPotassium) {
                growModel.Stress.AddOrSum(StressType.ConsumptionPotassium, consumptionDescriptor.StressGainPotassium);
            }

            if (!soilConsumptionModel.HasEnoughPhosphorus) {
                growModel.Stress.AddOrSum(StressType.ConsumptionPhosphorus, consumptionDescriptor.StressGainPhosphorus);
            }

            if (soilConsumptionModel.HasNothing()) {
                growModel.Stress.AddOrSum(StressType.ConsumptionOverall, consumptionDescriptor.NotEnoughStressGain);
            }
        }

        private SoilConsumptionModel CreateSoilConsumptionModel(SoilModel soilModel,
                                                                float nitrogenUsage,
                                                                float potassiumUsage,
                                                                float phosphorusUsage,
                                                                float waterUsage)
        {
            bool hasWater = soilModel.WaterAmount >= waterUsage;
            bool hasNitrogen = soilModel.Elements.Nitrogen >= nitrogenUsage;
            bool hasPotassium = soilModel.Elements.Potassium >= potassiumUsage;
            bool hasPhosphorus = soilModel.Elements.Phosphorus >= phosphorusUsage;

            float humusUsage = 0f;
            if (!hasNitrogen) {
                humusUsage += nitrogenUsage;
            }
            if (!hasPotassium) {
                humusUsage += potassiumUsage;
            }
            if (!hasPhosphorus) {
                humusUsage += phosphorusUsage;
            }

            bool canUseHumus = soilModel.Humus >= humusUsage;
            ElementsModel elementsModel = canUseHumus
                                                  ? new(hasNitrogen ? nitrogenUsage : 0f, hasPotassium ? potassiumUsage : 0f,
                                                        hasPhosphorus ? phosphorusUsage : 0f)
                                                  : new ElementsModel(0f, 0f, 0f);

            return new(elementsModel, waterUsage, humusUsage, hasWater, hasNitrogen, hasPotassium, hasPhosphorus);
        }

        private void CalculateSunlightAffect(PlantSunlightParameters sunlightParameters, ref PlantGrowCalculationModel calculationModel)
        {
            float currentSunlight = _sunlightService.GetDailySunAmount();
            Debug.LogWarning($"Current sunlight for plant is = {currentSunlight}");
            if (sunlightParameters.MinSunlight > currentSunlight) {
                float deviation = currentSunlight - sunlightParameters.MinSunlight;
                calculationModel.Damage += sunlightParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.LowSunlight, sunlightParameters.StressGain * deviation);
            }

            if (sunlightParameters.MaxSunlight < currentSunlight) {
                float deviation = sunlightParameters.MaxSunlight - currentSunlight;
                calculationModel.Damage += sunlightParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.HighSunlight, sunlightParameters.StressGain * deviation);
            }

            if (sunlightParameters.MinPreferredSunlight < currentSunlight && sunlightParameters.MaxPreferredSunlight > currentSunlight) {
                calculationModel.GrowMultiplier += sunlightParameters.GrowBuff;
            }
        }

        private void CalculatePhAffect(PlantPhParameters phParameters, SoilModel soilModel, ref PlantGrowCalculationModel calculationModel)
        {
            float currentPh = soilModel.Ph;
            if (phParameters.MinPh > currentPh) {
                float deviation = currentPh - phParameters.MinPh;
                calculationModel.Damage += phParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.LowPh, phParameters.StressGain * deviation);
            }

            if (phParameters.MaxPh < currentPh) {
                float deviation = phParameters.MaxPh - currentPh;
                calculationModel.Damage += phParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.HighPh, phParameters.StressGain * deviation);
            }

            if (phParameters.MinPreferredPh < currentPh && phParameters.MaxPreferredPh > currentPh) {
                calculationModel.GrowMultiplier += phParameters.GrowBuff;
            }
        }

        private void CalculateTemperatureAffect(PlantTemperatureParameters temperatureParameters, ref PlantGrowCalculationModel calculationModel)
        {
            TemperatureModel currentTemperatureModel = _temperatureService.GetTemperatureModel();

            float maxDayTemperature = currentTemperatureModel.DayTemperature;
            float minNightTemperature = currentTemperatureModel.NightTemperature;
            float averageTemperature = currentTemperatureModel.AverageTemperature;

            if (temperatureParameters.MinTemperature > minNightTemperature) {
                float deviation = temperatureParameters.MinTemperature - minNightTemperature;
                calculationModel.Damage += temperatureParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.LowTemperature, temperatureParameters.StressGain * deviation);
            }

            if (temperatureParameters.MaxTemperature < maxDayTemperature) {
                float deviation = maxDayTemperature - temperatureParameters.MaxTemperature;
                calculationModel.Damage += temperatureParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.HighTemperature, temperatureParameters.StressGain * deviation);
            }

            if (temperatureParameters.MinPreferredTemperature < averageTemperature
                && temperatureParameters.MaxPreferredTemperature > averageTemperature) {
                calculationModel.GrowMultiplier += temperatureParameters.GrowBuff;
            }
        }

        private void CalculateAirHumidityAffect(PlantHumidityParameters airHumidityParameters, ref PlantGrowCalculationModel calculationModel)
        {
            float airHumidityPercent = _airHumidityService.GetDailyAirHumidity();

            Debug.Log($"Air humidity affect is = {airHumidityPercent}");

            if (airHumidityParameters.MinHumidity > airHumidityPercent) {
                float deviation = airHumidityParameters.MinHumidity - airHumidityPercent;
                calculationModel.Damage += airHumidityParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.LowAirHumidity, airHumidityParameters.StressGain * deviation);
            }

            if (airHumidityParameters.MaxHumidity < airHumidityPercent) {
                float deviation = airHumidityPercent - airHumidityParameters.MaxHumidity;
                calculationModel.Damage += airHumidityParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.HighAirHumidity, airHumidityParameters.StressGain * deviation);
            }

            if (airHumidityParameters.MinPreferredHumidity < airHumidityPercent && airHumidityParameters.MaxPreferredHumidity > airHumidityPercent) {
                calculationModel.GrowMultiplier += airHumidityParameters.GrowBuff;
            }
        }

        private void CalculateSoilHumidityAffect(PlantHumidityParameters soilHumidityParameters,
                                                 SoilModel soilModel,
                                                 ref PlantGrowCalculationModel calculationModel)
        {
            float soilHumidity = soilModel.SoilHumidity;
            if (soilHumidityParameters.MinHumidity > soilHumidity) {
                float deviation = soilHumidityParameters.MinHumidity - soilHumidity;
                calculationModel.Damage += soilHumidityParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.LowSoilHumidity, soilHumidityParameters.StressGain * deviation);
            }

            if (soilHumidityParameters.MaxHumidity < soilHumidity) {
                float deviation = soilHumidity - soilHumidityParameters.MaxHumidity;
                calculationModel.Damage += soilHumidityParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.HighSoilHumidity, soilHumidityParameters.StressGain * deviation);
            }

            if (soilHumidityParameters.MinPreferredHumidity < soilHumidity && soilHumidityParameters.MaxPreferredHumidity > soilHumidity) {
                calculationModel.GrowMultiplier += soilHumidityParameters.GrowBuff;
            }
        }

        private void CalculateSalinityAffect(PlantSalinityParameters salinityParameters,
                                             SoilModel soilModel,
                                             ref PlantGrowCalculationModel calculationModel)
        {
            float soilHumidity = soilModel.Salinity;
            if (salinityParameters.MinSalinity > soilHumidity) {
                float deviation = salinityParameters.MinSalinity - soilHumidity;
                calculationModel.Damage += salinityParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.LowSalinity, salinityParameters.StressGain * deviation);
            }

            if (salinityParameters.MaxSalinity < soilHumidity) {
                float deviation = soilHumidity - salinityParameters.MaxSalinity;
                calculationModel.Damage += salinityParameters.DamagePerDeviation * deviation;
                calculationModel.Stress.TryAdd(StressType.HighSalinity, salinityParameters.StressGain * deviation);
            }
        }
    }
}