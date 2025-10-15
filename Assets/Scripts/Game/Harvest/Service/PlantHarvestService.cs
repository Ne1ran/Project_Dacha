using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Common.Descriptor;
using Game.Harvest.Descriptor;
using Game.Harvest.Model;
using Game.Harvest.PlantHarvest;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Soil.Model;
using UnityEngine;

namespace Game.Harvest.Service
{
    [Service]
    public class PlantHarvestService
    {
        private readonly PlantHarvestDescriptor _plantHarvestDescriptor;
        private readonly PlantHarvestHandlerFactory _plantHarvestHandlerFactory;

        public PlantHarvestService(PlantHarvestHandlerFactory plantHarvestHandlerFactory, PlantHarvestDescriptor plantHarvestDescriptor)
        {
            _plantHarvestHandlerFactory = plantHarvestHandlerFactory;
            _plantHarvestDescriptor = plantHarvestDescriptor;
        }

        public void SimulateHarvestGrowth(ref PlantModel plantModel,
                                          PlantsDescriptorModel plantsDescriptorModel,
                                          PlantGrowCalculationModel growModel,
                                          int dayDifference)
        {
            string harvestDescriptorId = plantsDescriptorModel.HarvestDescriptorId;

            PlantHarvestDescriptorModel harvestDescriptorModel = _plantHarvestDescriptor.Require(harvestDescriptorId);

            if (!growModel.BlockHarvestGrowth) {
                TryGrowHarvest(plantModel, harvestDescriptorModel, growModel.GrowMultiplier, dayDifference);
            }

            if (plantModel.CurrentStage != harvestDescriptorModel.HarvestSpawnStage) {
                return;
            }

            if (!growModel.BlockNewHarvestSpawn) {
                TryAddNewHarvest(plantModel, harvestDescriptorModel, harvestDescriptorId);
            }
        }

        public SoilConsumptionModel GetHarvestConsumption(PlantModel plantModel, float growMultiplier, int dayDifference)
        {
            SoilConsumptionModel soilConsumptionModel = new();

            foreach (PlantHarvestModel harvest in plantModel.Harvest) {
                PlantHarvestDescriptorModel harvestDescriptorModel = _plantHarvestDescriptor.Require(harvest.HarvestId);
                HarvestGrowStage currentStage = harvest.Stage;
                PlantHarvestGrowDescriptor harvestStageDescriptor = harvestDescriptorModel.HarvestGrowStages[currentStage];
                float neededProgress = harvestStageDescriptor.NextStageGrowDays;

                float stageGrowth = dayDifference * growMultiplier;
                float usedElementsProportion = stageGrowth / neededProgress;
                ConsumptionDescriptor harvestConsumption = harvestStageDescriptor.HarvestConsumption;
                float nitrogenUsage = harvestConsumption.NitrogenUsage * usedElementsProportion;
                float potassiumUsage = harvestConsumption.PotassiumUsage * usedElementsProportion;
                float phosphorusUsage = harvestConsumption.PhosphorusUsage * usedElementsProportion;
                float waterUsage = harvestConsumption.WaterUsage * usedElementsProportion;
                soilConsumptionModel.Add(new(nitrogenUsage, potassiumUsage, phosphorusUsage), waterUsage);
            }

            return soilConsumptionModel;
        }

        public List<HarvestModel> GetAllHarvestFromPlant(PlantModel plantModel)
        {
            List<HarvestModel> harvest = new();

            foreach (PlantHarvestModel harvestModel in plantModel.Harvest) {
                PlantHarvestDescriptorModel plantHarvestDescriptorModel = _plantHarvestDescriptor.Require(harvestModel.HarvestId);
                PlantHarvestGrowDescriptor harvestGrowStage = plantHarvestDescriptorModel.HarvestGrowStages[harvestModel.Stage];
                HarvestQuality currentQuality = harvestGrowStage.CurrentQuality;
                if (currentQuality != HarvestQuality.None) {
                    harvest.Add(new(plantHarvestDescriptorModel.HarvestItemId, currentQuality, 1));
                }
            }

            return harvest;
        }

        private void TryGrowHarvest(PlantModel plantModel,
                                    PlantHarvestDescriptorModel harvestDescriptorModel,
                                    float growMultiplier,
                                    int dayDifference)
        {
            foreach (PlantHarvestModel harvest in plantModel.Harvest) {
                HarvestGrowStage currentStage = harvest.Stage;
                PlantHarvestGrowDescriptor stageDescriptor = harvestDescriptorModel.HarvestGrowStages[currentStage];

                float currentProgress = harvest.Progress;
                float neededProgress = stageDescriptor.NextStageGrowDays;
                if (currentProgress < neededProgress) {
                    harvest.Progress += growMultiplier * dayDifference;
                    continue;
                }

                UpdateHarvestStage(harvestDescriptorModel, currentStage, harvest);
            }
        }

        private void UpdateHarvestStage(PlantHarvestDescriptorModel harvestDescriptorModel, HarvestGrowStage currentStage, PlantHarvestModel harvest)
        {
            bool takeNextStage = false;
            HarvestGrowStage newStage = currentStage;
            foreach (HarvestGrowStage stage in harvestDescriptorModel.HarvestGrowStages.Keys) {
                if (stage == currentStage) {
                    takeNextStage = true;
                    continue;
                }

                if (takeNextStage) {
                    newStage = stage;
                    break;
                }
            }

            if (newStage == currentStage) {
                return;
            }

            harvest.Progress = 0;
            harvest.Stage = newStage;
        }

        private void TryAddNewHarvest(PlantModel plantModel, PlantHarvestDescriptorModel harvestDescriptorModel, string harvestDescriptorId)
        {
            int currentHarvest = plantModel.Harvest.Count;
            float harvestAmountMultiplier = 1f;
            if (currentHarvest > harvestDescriptorModel.MaxHarvestPoint) {
                return;
            }

            if (currentHarvest > harvestDescriptorModel.TooManyHarvestPoint) {
                int deviation = currentHarvest - harvestDescriptorModel.TooManyHarvestPoint;
                harvestAmountMultiplier = harvestDescriptorModel.TooManyHarvestDebuffPerDeviation / deviation;
            }

            float healthHarvestMultiplier = plantModel.Health * harvestDescriptorModel.MultiplierPerHealth;
            float possibleHarvest = harvestDescriptorModel.HarvestSpawnPerDay * harvestAmountMultiplier * healthHarvestMultiplier;
            int harvestToRandom = Mathf.FloorToInt(possibleHarvest);
            int actualNewHarvest = 0;
            for (int i = 0; i < harvestToRandom; i++) {
                float roll = Random.Range(0f, 1f);
                if (roll < harvestDescriptorModel.HarvestSpawnChance) {
                    actualNewHarvest++;
                }
            }

            for (int i = 0; i < actualNewHarvest; i++) {
                plantModel.Harvest.Add(new(harvestDescriptorId, 0f, harvestDescriptorModel.HarvestGrowStages.First().Key));
            }
        }
    }
}