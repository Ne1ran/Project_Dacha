using System;
using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Calendar.Event;
using Game.Diseases.Model;
using Game.Diseases.Service;
using Game.Plants.Descriptors;
using Game.Plants.Event;
using Game.Plants.Model;
using Game.Plants.PlantParams;
using Game.Plants.Repo;
using Game.Seeds.Descriptors;
using Game.Soil.Model;
using Game.Soil.Service;
using Game.Stress.Descriptor;
using Game.Stress.Model;
using Game.Symptoms.Descriptor;
using Game.Utils;
using MessagePipe;
using UnityEngine;

namespace Game.Plants.Service
{
    [Service]
    public class PlantsService : IDisposable
    {
        private readonly PlantsRepo _plantsRepo;
        private readonly PlantsParametersHandlerFactory _plantsParametersHandlerFactory;
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
                             IPublisher<string, PlantUpdatedEvent> plantUpdatedEvent,
                             PlantsParametersHandlerFactory plantsParametersHandlerFactory)
        {
            _plantsRepo = plantsRepo;
            _descriptorService = descriptorService;
            _soilService = soilService;
            _plantDiseaseService = plantDiseaseService;
            _plantUpdatedEvent = plantUpdatedEvent;
            _plantsParametersHandlerFactory = plantsParametersHandlerFactory;

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

        public PlantInspectionModel? InspectPlant(string tileId)
        {
            if (!_plantsRepo.Exists(tileId)) {
                Debug.LogWarning($"Plant doesn't exists on tile={tileId}");
                return null;
            }

            PlantModel plantModel = _plantsRepo.Require(tileId);

            if (plantModel.InspectedToday) {
                Debug.Log($"No need to double inspect plant on tile={tileId}");
                return new(plantModel);
            }

            if (plantModel.CurrentStage == PlantGrowStage.SEED) {
                Debug.Log($"No need to inspect plant in seeds for now on tile={tileId}");
                return new(plantModel);
            }

            if (plantModel.DiseaseModels.Count > 0) {
                _plantDiseaseService.CheckSymptoms(plantModel);
            }

            CheckStressSymptoms(plantModel);
            plantModel.InspectedToday = true;
            return new(plantModel);
        }

        private void CheckStressSymptoms(PlantModel plantModel)
        {
            StressDescriptor stressDescriptor = _descriptorService.Require<StressDescriptor>();
            SymptomsDescriptor symptomsDescriptor = _descriptorService.Require<SymptomsDescriptor>();
            foreach ((StressType stressType, StressModel stressModel) in plantModel.Stress) {
                if (!stressDescriptor.Items.TryGetValue(stressType, out StressModelDescriptor stressDescriptorItem)) {
                    continue;
                }

                if (stressDescriptorItem.SymptomsShowThreshold > stressModel.StressValue) {
                    continue;
                }

                float showSymptomRoll = UnityEngine.Random.Range(0f, 1f);
                if (showSymptomRoll < stressDescriptorItem.SymptomShowChance) {
                    continue;
                }

                List<string> allSymptoms = stressDescriptorItem.Symptoms;
                if (stressDescriptorItem.PlantFamilySymptoms.TryGetValue(plantModel.FamilyType, out List<string> plantFamilySymptoms)) {
                    allSymptoms.AddRange(plantFamilySymptoms);
                }

                List<string> possibleSymptoms = GetPossibleSymptoms(ref allSymptoms, symptomsDescriptor, plantModel.FamilyType);
                if (possibleSymptoms.Count == 0) {
                    continue;
                }

                string selectedSymptom = possibleSymptoms.PickRandom();
                stressModel.StressSymptoms.Add(selectedSymptom);
            }
        }

        private List<string> GetPossibleSymptoms(ref List<string> allSymptoms, SymptomsDescriptor symptomsDescriptor, PlantFamilyType plantFamilyType)
        {
            for (int i = 0; i < allSymptoms.Count; i++) {
                string symptom = allSymptoms[i];
                if (!symptomsDescriptor.Items.TryGetValue(symptom, out SymptomDescriptorModel descriptorModel)) {
                    allSymptoms.Remove(symptom);
                    continue;
                }

                if (descriptorModel.BannedFamilyTypes.Contains(plantFamilyType)) {
                    allSymptoms.Remove(symptom);
                }
            }

            return allSymptoms;
        }

        public void RemovePlant(string tileId)
        {
            PlantModel? plantModel = _plantsRepo.Get(tileId);
            if (plantModel == null) {
                return;
            }

            List<SavedDiseaseModel> savedDiseaseModels = _plantDiseaseService.GetSavedDiseases(plantModel);
            _soilService.InfectSoil(tileId, savedDiseaseModels);
            _soilService.RemovePlant(tileId);
            _plantsRepo.Delete(tileId);
            _plantUpdatedEvent.Publish(PlantUpdatedEvent.Removed, new(tileId, plantModel));
        }

        public void CreatePlantFromSeed(string seedId, string tileId)
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

            CreatePlant(seedsDescriptorModel.PlantId, tileId, seedsDescriptorModel.StartHealth, seedsDescriptorModel.StartImmunity);
        }

        public void CreatePlant(string plantId, string tileId, float startHealth, float startImmunity)
        {
            PlantsDescriptor plantsDescriptor = _descriptorService.Require<PlantsDescriptor>();
            PlantsDescriptorModel? plantsDescriptorModel = plantsDescriptor.Items.Find(plant => plant.Id == plantId);
            if (plantsDescriptorModel == null) {
                Debug.LogWarning($"Plant not found plantId={plantId}");
                return;
            }

            PlantModel plantModel = new(plantId, plantsDescriptorModel.FamilyType, PlantGrowStage.SEED, startHealth, startImmunity);
            _plantUpdatedEvent.Publish(PlantUpdatedEvent.Created, new(tileId, plantModel));
            _plantsRepo.Save(tileId, plantModel);
        }

        private void OnDayFinished(DayChangedEvent evt)
        {
            Dictionary<string, PlantModel> plants = _plantsRepo.GetAll();

            PlantsDescriptor plantsDescriptor = _descriptorService.Require<PlantsDescriptor>();
            foreach ((string tileId, PlantModel plant) in plants) {
                try {
                    PlantModel plantModel = plant;
                    PlantsDescriptorModel plantsDescriptorModel = plantsDescriptor.RequirePlant(plantModel.PlantId);
                    SimulatePlantLife(ref plantModel, tileId, plantsDescriptorModel, evt.DayDifference);
                    if (plantModel.CurrentStage != PlantGrowStage.DEAD) {
                        _plantDiseaseService.UpdatePlantDiseases(ref plantModel, plantsDescriptorModel, tileId);
                    }
                    _plantUpdatedEvent.Publish(PlantUpdatedEvent.Updated, new(tileId, plantModel));
                } catch (Exception e) {
                    Debug.LogWarning($"Exception when simulating plant life. TileId={tileId}, plantId={plant.PlantId}");
                    Debug.LogException(e);
                }
            }
        }

        private void SimulatePlantLife(ref PlantModel plant, string soilId, PlantsDescriptorModel plantsDescriptorModel, int dayDifference)
        {
            PlantGrowStage plantCurrentStage = plant.CurrentStage;
            if (plant.CurrentStage == PlantGrowStage.DEAD) {
                return;
            }

            PlantStageDescriptor plantStageDescriptor = plantsDescriptorModel.RequireStage(plantCurrentStage);
            PlantGrowCalculationModel growModel = GetGrowModel(plant, soilId, dayDifference, plantsDescriptorModel, plantStageDescriptor);
            ApplyGrowCalculationModel(plant, soilId, growModel);
            Debug.Log($"Plant life simulation. GrowCalcModel: growMultiplier={growModel.GrowMultiplier}, damage={growModel.Damage}");
            if (plant.CurrentStage == PlantGrowStage.DEAD) {
                Debug.LogWarning($"Plant have died. Damage={growModel.Damage}, StressReasons={string.Join(", ", growModel.Stress.Keys.ToList())}");
                // if plant have died - no need for further calculations 
                return;
            }

            if (!growModel.BlockHealing) {
                TryHealPlant(ref plant, plantStageDescriptor, soilId);
            }

            if (!growModel.BlockImmunityGain) {
                TryIncreaseImmunity(ref plant, plantStageDescriptor);
            }

            TryLowerStress(ref plant, plantStageDescriptor);
            TryGrowToNextStage(ref plant, plantStageDescriptor, plantsDescriptorModel);
        }

        private void TryLowerStress(ref PlantModel plant, PlantStageDescriptor stageDescriptor)
        {
            List<StressType> plantStressTypes = plant.Stress.Keys.ToList();
            foreach (StressType stressType in plantStressTypes) {
                plant.Stress[stressType].StressValue -= stageDescriptor.DailyStressDecrease;
            }

            List<StressType> stressToRemove = new();
            foreach ((StressType stressType, StressModel stressModel) in plant.Stress) {
                if (stressModel.StressValue <= 0f) {
                    stressToRemove.Add(stressType);
                }
            }

            foreach (StressType stressType in stressToRemove) {
                plant.Stress.Remove(stressType);
            }
        }

        private PlantGrowCalculationModel GetGrowModel(PlantModel plant,
                                                       string soilId,
                                                       int dayDifference,
                                                       PlantsDescriptorModel plantsDescriptorModel,
                                                       PlantStageDescriptor plantStageDescriptor)
        {
            PlantGrowCalculationModel growModel = new();
            SoilModel soilModel = _soilService.RequireSoil(soilId);
            float randomGrowthNoise = UnityEngine.Random.Range(-plantsDescriptorModel.PlantGrowNoise, plantsDescriptorModel.PlantGrowNoise);
            growModel.GrowMultiplier = Mathf.Max(growModel.GrowMultiplier + randomGrowthNoise, 0f);

            List<IPlantParameters> parametersList = new();
            parametersList.Add(plantsDescriptorModel.PhParameters);
            if (plantStageDescriptor.IncludeSunlight) {
                parametersList.Add(plantStageDescriptor.SunlightParameters);
            }

            if (plantStageDescriptor.IncludeTemperature) {
                parametersList.Add(plantStageDescriptor.TemperatureParameters);
            }

            if (plantStageDescriptor.IncludeAirHumidity) {
                parametersList.Add(plantStageDescriptor.AirSoilHumidityParameters);
            }

            if (plantStageDescriptor.IncludeSoilHumidity) {
                parametersList.Add(plantStageDescriptor.SoilSoilHumidityParameters);
            }

            if (plantStageDescriptor.IncludeSalinity) {
                parametersList.Add(plantStageDescriptor.SalinityParameters);
            }

            foreach (IPlantParameters plantParameters in parametersList) {
                _plantsParametersHandlerFactory.Create(plantParameters.ParametersType.ToString())
                                               .ApplyParameters(plantParameters, growModel, soilModel);
            }

            CalculateConsumption(plantStageDescriptor.PlantConsumption, soilModel, plantStageDescriptor.AverageGrowTime, dayDifference,
                                 ref growModel);

            if (plant.CurrentStage != PlantGrowStage.SEED) {
                ApplyStress(plant, plantsDescriptorModel.StressParameters, ref growModel);
            }

            return growModel;
        }

        private void ApplyStress(PlantModel plant, PlantStressParameters stressParameters, ref PlantGrowCalculationModel growModel)
        {
            foreach ((StressType stressType, float stressAmount) in growModel.Stress) {
                plant.AddStress(stressType, stressAmount, stressParameters.MaxStress);
            }

            if (plant.Stress.Count == 0) {
                return;
            }

            float maxStress = 0;
            foreach (StressModel stressModel in plant.Stress.Values) {
                if (stressModel.StressValue > maxStress) {
                    maxStress = stressModel.StressValue;
                }
            }

            float blockHealingThreshold = stressParameters.MaxStress * stressParameters.BlockHealingThreshold;
            float blockImmunityGainThreshold = stressParameters.MaxStress * stressParameters.BlockImmunityGainThreshold;
            float blockGrowthThreshold = stressParameters.MaxStress * stressParameters.BlockGrowthThreshold;
            float damageThreshold = stressParameters.MaxStress * stressParameters.DealDamageThreshold;

            growModel.BlockHealing = maxStress > blockHealingThreshold;
            growModel.BlockImmunityGain = maxStress > blockImmunityGainThreshold;
            growModel.BlockGrowth = maxStress > blockGrowthThreshold;

            if (maxStress > damageThreshold) {
                growModel.Damage += maxStress * stressParameters.StressDamageMultiplier;
            }
        }

        private void TryHealPlant(ref PlantModel plant, PlantStageDescriptor plantStageDescriptor, string soilId)
        {
            if (plant.Health >= Constants.Constants.MaxPlantHealth) {
                return;
            }

            float neededHealth = Mathf.Min(plantStageDescriptor.DailyRegeneration, Constants.Constants.MaxPlantHealth - plant.Health);
            if (_soilService.TryConsumeHumus(soilId, neededHealth)) {
                plant.Health = Mathf.Clamp(plant.Health + neededHealth, 0f, Constants.Constants.MaxPlantHealth);
            }
        }

        private void TryIncreaseImmunity(ref PlantModel plant, PlantStageDescriptor plantStageDescriptor)
        {
            if (plant.Immunity >= Constants.Constants.MaxImmunity) {
                return;
            }

            float healthMultiplier = plant.Health / Constants.Constants.MaxPlantHealth;
            plant.Immunity = Mathf.Clamp(plant.Immunity + healthMultiplier * plantStageDescriptor.DailyImmunityGain, 0f,
                                         Constants.Constants.MaxImmunity);
        }

        private void ApplyGrowCalculationModel(PlantModel plant, string soilId, PlantGrowCalculationModel calculationModel)
        {
            if (calculationModel.Damage > 0) {
                float decreaseImmunity = plant.DecreaseImmunity(calculationModel.Damage);
                float damageChance = Mathf.Clamp(decreaseImmunity / 100f, 0.05f, 0.95f);
                float damageRoll = UnityEngine.Random.Range(0f, 1f);
                if (damageRoll > damageChance) {
                    plant.DealDamage(calculationModel.Damage);
                }
            }

            ConsumeElements(plant, soilId, calculationModel.Consumption);

            if (!calculationModel.BlockGrowth) {
                plant.StageGrowth += Mathf.Max(calculationModel.ActualGrowth, 0f);
            }
        }

        private void TryGrowToNextStage(ref PlantModel plant,
                                        PlantStageDescriptor currentStageDescriptor,
                                        PlantsDescriptorModel plantsDescriptorModel)
        {
            if (plant.StageGrowth < currentStageDescriptor.AverageGrowTime) {
                return;
            }

            int stageIndex = plantsDescriptorModel.Stages.IndexOf(currentStageDescriptor);
            int newStageIndex = stageIndex + 1;
            if (newStageIndex >= plantsDescriptorModel.Stages.Count) {
                return;
            }

            PlantStageDescriptor newStageDescriptor = plantsDescriptorModel.Stages[newStageIndex];
            plant.CurrentStage = newStageDescriptor.Stage;
            plant.StageGrowth = 0f;
            Debug.Log($"Plant has grew to next stage! PlantId={plant.PlantId}, newStage={newStageDescriptor.Stage}");
        }

        private void ConsumeElements(PlantModel plant, string soilId, SoilConsumptionModel consumptionModel)
        {
            _soilService.ConsumeForPlant(soilId, consumptionModel.WaterUsage, consumptionModel.ElementsUsage, consumptionModel.HumusUsage);
            plant.TakenElements.Add(consumptionModel.ElementsUsage);
        }

        private void CalculateConsumption(PlantConsumptionDescriptor consumptionDescriptor,
                                          SoilModel soilModel,
                                          float growTime,
                                          int dayDifference,
                                          ref PlantGrowCalculationModel growModel)
        {
            float dayCoeff = (dayDifference / growTime) * growModel.GrowMultiplier;
            float nitrogenUsage = consumptionDescriptor.NitrogenUsage * dayCoeff;
            float potassiumUsage = consumptionDescriptor.PotassiumUsage * dayCoeff;
            float phosphorusUsage = consumptionDescriptor.PhosphorusUsage * dayCoeff;
            float waterUsage = consumptionDescriptor.WaterUsage * dayCoeff;
            growModel.ActualGrowth = dayDifference * growModel.GrowMultiplier;
            growModel.Consumption = CreateSoilConsumptionModel(soilModel, nitrogenUsage, potassiumUsage, phosphorusUsage, waterUsage,
                                                               out bool hasEnoughWater, out bool hasEnoughNitrogen, out bool hasEnoughPotassium,
                                                               out bool hasEnoughPhosphorus, out bool canConsumeElements);

            if (!hasEnoughWater) {
                growModel.Stress.AddOrSum(StressType.ConsumptionWater, consumptionDescriptor.StressGainWater);
            }

            if (!hasEnoughNitrogen) {
                growModel.Stress.AddOrSum(StressType.ConsumptionNitrogen, consumptionDescriptor.StressGainNitrogen);
            }

            if (!hasEnoughPotassium) {
                growModel.Stress.AddOrSum(StressType.ConsumptionPotassium, consumptionDescriptor.StressGainPotassium);
            }

            if (!hasEnoughPhosphorus) {
                growModel.Stress.AddOrSum(StressType.ConsumptionPhosphorus, consumptionDescriptor.StressGainPhosphorus);
            }

            if (!canConsumeElements) {
                growModel.Stress.AddOrSum(StressType.ConsumptionOverall, consumptionDescriptor.NotEnoughElementsStressGain);
            }
        }

        private SoilConsumptionModel CreateSoilConsumptionModel(SoilModel soilModel,
                                                                float nitrogenUsage,
                                                                float potassiumUsage,
                                                                float phosphorusUsage,
                                                                float waterUsage,
                                                                out bool hasEnoughWater,
                                                                out bool hasEnoughNitrogen,
                                                                out bool hasEnoughPotassium,
                                                                out bool hasEnoughPhosphorus,
                                                                out bool canConsumeElements)
        {
            hasEnoughWater = soilModel.WaterAmount >= waterUsage;
            hasEnoughNitrogen = soilModel.Elements.Nitrogen >= nitrogenUsage;
            hasEnoughPotassium = soilModel.Elements.Potassium >= potassiumUsage;
            hasEnoughPhosphorus = soilModel.Elements.Phosphorus >= phosphorusUsage;

            float humusUsage = 0f;
            if (!hasEnoughNitrogen) {
                humusUsage += nitrogenUsage;
            }
            if (!hasEnoughPotassium) {
                humusUsage += potassiumUsage;
            }
            if (!hasEnoughPhosphorus) {
                humusUsage += phosphorusUsage;
            }

            ElementsModel elementsModel;
            bool allElementsPersist = hasEnoughNitrogen && hasEnoughPotassium && hasEnoughPhosphorus;
            if (allElementsPersist) {
                canConsumeElements = true;
                elementsModel = new(nitrogenUsage, potassiumUsage, phosphorusUsage);
            } else {
                canConsumeElements = soilModel.Humus >= humusUsage;
                elementsModel = new(hasEnoughNitrogen ? nitrogenUsage : 0f, hasEnoughPotassium ? potassiumUsage : 0f,
                                    hasEnoughPhosphorus ? phosphorusUsage : 0f);
            }

            return new(elementsModel, hasEnoughWater ? waterUsage : 0f, humusUsage);
        }
    }
}