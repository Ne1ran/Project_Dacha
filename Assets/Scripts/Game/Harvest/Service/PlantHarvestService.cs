using System.Linq;
using Core.Attributes;
using Core.Common.Descriptor;
using Core.Descriptors.Service;
using Game.Harvest.Descriptor;
using Game.Harvest.Model;
using Game.Harvest.PlantHarvest;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Soil.Model;
using Game.Soil.Service;
using UnityEngine;

namespace Game.Harvest.Service
{
    [Service]
    public class PlantHarvestService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly PlantHarvestHandlerFactory _plantHarvestHandlerFactory;
        private readonly SoilService _soilService;

        public PlantHarvestService(IDescriptorService descriptorService,
                                   PlantHarvestHandlerFactory plantHarvestHandlerFactory,
                                   SoilService soilService)
        {
            _descriptorService = descriptorService;
            _plantHarvestHandlerFactory = plantHarvestHandlerFactory;
            _soilService = soilService;
        }

        public void SimulateHarvestGrowth(ref PlantModel plantModel,
                                          PlantsDescriptorModel plantsDescriptorModel,
                                          PlantGrowCalculationModel growModel,
                                          string soilId)
        {
            string harvestDescriptorId = plantsDescriptorModel.HarvestDescriptorId;
            PlantHarvestDescriptor harvestDescriptor = _descriptorService.Require<PlantHarvestDescriptor>();
            PlantHarvestDescriptorModel harvestDescriptorModel = harvestDescriptor.Require(harvestDescriptorId);
            if (plantModel.CurrentStage != harvestDescriptorModel.HarvestSpawnStage) {
                return;
            }

            TryGrowHarvest(plantModel, harvestDescriptorModel, growModel.GrowMultiplier);

            if (!growModel.BlockHarvestGrowth) {
                TryAddNewHarvest(plantModel, harvestDescriptorModel, harvestDescriptorId);
            }
        }

        public SoilConsumptionModel GetHarvestConsumption(PlantModel plantModel, PlantsDescriptorModel plantsDescriptorModel)
        {
            SoilConsumptionModel soilConsumptionModel = new();

            foreach (var VARIABLE in plantModel.Harvest) {
                
            }

            // todo neiran Подумай как правильно добавить забирание питания на урожай. Учти, что мы считаем growMultiplier из растения. Т.е. вызовы как будто должны быть разных местах?...
            
            return soilConsumptionModel;
        }

        private void TryGrowHarvest(PlantModel plantModel, PlantHarvestDescriptorModel harvestDescriptorModel, float growMultiplier)
        {
            foreach (PlantHarvestModel harvest in plantModel.Harvest) {
                HarvestGrowStage currentStage = harvest.Stage;
                PlantHarvestGrowDescriptor stageDescriptor = harvestDescriptorModel.HarvestGrowStages[currentStage];

                float currentProgress = harvest.Progress;
                float neededProgress = stageDescriptor.NextStageGrowDays;
                if (currentProgress > neededProgress) {
                    harvest.Progress += growMultiplier * 1f;
                    break;
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
                plantModel.Harvest.Add(new(harvestDescriptorId, 1f, harvestDescriptorModel.HarvestGrowStages.First().Key));
            }
        }
    }
}