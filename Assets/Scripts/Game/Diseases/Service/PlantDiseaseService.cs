using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Diseases.Descriptor;
using Game.Diseases.Model;
using Game.GameMap.Tiles.Service;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Plants.Repo;
using Game.Soil.Service;
using Game.Utils;
using UnityEngine;

namespace Game.Diseases.Service
{
    [Service]
    public class PlantDiseaseService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly PlantsRepo _plantsRepo;
        private readonly SoilService _soilService;
        private readonly WorldTileService _worldTileService;

        public PlantDiseaseService(IDescriptorService descriptorService,
                                   PlantsRepo plantsRepo,
                                   WorldTileService worldTileService,
                                   SoilService soilService)
        {
            _descriptorService = descriptorService;
            _plantsRepo = plantsRepo;
            _worldTileService = worldTileService;
            _soilService = soilService;
        }

        public void UpdatePlantDiseases(ref PlantModel plant, PlantsDescriptorModel plantsDescriptorModel, string soilId)
        {
            DiseasesDescriptor diseasesDescriptor = _descriptorService.Require<DiseasesDescriptor>();
            TryIncreaseDiseasesGrowth(ref plant, diseasesDescriptor, soilId);
            TryGetInfected(ref plant, plantsDescriptorModel, soilId, diseasesDescriptor);
        }

        public void CheckSymptoms(PlantModel plantModel)
        {
            DiseasesDescriptor diseasesDescriptor = _descriptorService.Require<DiseasesDescriptor>();
            foreach (DiseaseModel disease in plantModel.DiseaseModels) {
                DiseaseModelDescriptor? diseaseModelDescriptor = diseasesDescriptor.Get(disease.Id);
                if (diseaseModelDescriptor == null) {
                    Debug.LogWarning($"Disease model descriptor not found with id={disease.Id}");
                    continue;
                }

                TryLearnRandomSymptom(disease, diseaseModelDescriptor);
            }
        }

        private void TryLearnRandomSymptom(DiseaseModel disease, DiseaseModelDescriptor diseaseModelDescriptor)
        {
            int currentStage = disease.Stage;
            InfectionStage stageDescriptor = diseaseModelDescriptor.InfectionModel.InfectionStages.Find(stage => stage.Stage == currentStage);

            Dictionary<string, bool> symptomsKnowledge = new();
            foreach (string randomSymptom in stageDescriptor.RandomSymptoms) {
                symptomsKnowledge.Add(randomSymptom, disease.KnownSymptoms.Contains(randomSymptom));
            }

            if (symptomsKnowledge.Values.All(value => value)) {
                return;
            }

            float symptomShowChance = disease.CurrentGrowth / stageDescriptor.StageGrowth;
            float random = Random.Range(0f, 1f);
            if (random >= symptomShowChance) {
                return;
            }

            List<string> list = new();
            foreach ((string symptom, bool known) in symptomsKnowledge) {
                if (known) {
                    continue;
                }

                list.Add(symptom);
            }

            string randomUnknownSymptom = list.PickRandom();
            disease.KnownSymptoms.Add(randomUnknownSymptom);
        }

        private void TryIncreaseDiseasesGrowth(ref PlantModel plant, DiseasesDescriptor diseasesDescriptor, string soilId)
        {
            for (int i = 0; i < plant.DiseaseModels.Count; i++) {
                DiseaseModel diseaseModel = plant.DiseaseModels[i];
                DiseaseModelDescriptor? diseaseModelDescriptor = diseasesDescriptor.Get(diseaseModel.Id);
                if (diseaseModelDescriptor == null) {
                    Debug.LogWarning($"Disease model descriptor not found with id={diseaseModel.Id}");
                    continue;
                }

                DiseaseInfectionModel diseaseInfectionModel = diseaseModelDescriptor.InfectionModel;
                int currentStage = diseaseModel.Stage;
                InfectionStage? infectionStage = diseaseInfectionModel.InfectionStages.Find(stage => stage.Stage == currentStage);
                if (infectionStage == null) {
                    Debug.LogWarning($"Infection stage not found. This shouldn't be possible. DiseaseId={diseaseModel.Id}, Stage={currentStage}");
                    continue;
                }

                bool result = TryHealDisease(plant, diseaseInfectionModel, infectionStage);
                if (result) {
                    plant.DiseaseModels.Remove(diseaseModel);
                }

                plant.DealDamage(infectionStage.PlantDamagePerDay);
                TryDiseaseGrowToNextStage(diseaseModel, infectionStage, diseaseInfectionModel);

                float diseaseGrowth = infectionStage.BaseGrowthPerDay;
                float growthMultiplier = CalculateGrowthMultiplier(diseaseInfectionModel, soilId);
                diseaseGrowth *= growthMultiplier;
                diseaseModel.CurrentGrowth += diseaseGrowth;
                Debug.Log($"Infection progression. PlantId={plant.PlantId}, diseaseId={diseaseModel.Id}, currentGrowth={diseaseModel.CurrentGrowth}");
            }
        }

        private bool TryHealDisease(PlantModel plant, DiseaseInfectionModel infectionModel, InfectionStage infectionStage)
        {
            if (!(plant.Health > infectionModel.HighHealth) || !(plant.Immunity > infectionModel.HighImmunity)) {
                return false;
            }

            float healChance = infectionStage.HealChance;
            float random = Random.Range(0f, 1f);
            return random < healChance;
        }

        private float CalculateGrowthMultiplier(DiseaseInfectionModel infectionModel, string soilId)
        {
            float multiplier = 1f;

            InfectionGrowthDescriptor infectionGrowthDescriptor = infectionModel.GrowthDescriptor;

            TryApplyTemperatureMultiplier(infectionGrowthDescriptor, ref multiplier);
            TryApplyAirHumidityMultiplier(infectionGrowthDescriptor, ref multiplier);
            TryApplySoilHumidityMultiplier(soilId, infectionGrowthDescriptor, ref multiplier);

            return multiplier;
        }

        private void TryApplySoilHumidityMultiplier(string soilId, InfectionGrowthDescriptor infectionGrowthDescriptor, ref float multiplier)
        {
            if (infectionGrowthDescriptor.IgnoreSoilHumidity) {
                return;
            }

            float soilHumidity = _soilService.GetSoilHumidity(soilId);
            if (infectionGrowthDescriptor.MinPreferredSoilHumidity < soilHumidity
                && infectionGrowthDescriptor.MaxPreferredSoilHumidity > soilHumidity) {
                multiplier += infectionGrowthDescriptor.SoilHumidityGrowBuff;
            }
        }

        private void TryApplyAirHumidityMultiplier(InfectionGrowthDescriptor infectionGrowthDescriptor, ref float multiplier)
        {
            if (infectionGrowthDescriptor.IgnoreAirHumidity) {
                return;
            }

            float airHumidity = 45f; // todo neiran add air service
            if (infectionGrowthDescriptor.MinPreferredAirHumidity < airHumidity && infectionGrowthDescriptor.MaxPreferredAirHumidity > airHumidity) {
                multiplier += infectionGrowthDescriptor.AirHumidityGrowBuff;
            }
        }

        private void TryApplyTemperatureMultiplier(InfectionGrowthDescriptor infectionGrowthDescriptor, ref float multiplier)
        {
            if (infectionGrowthDescriptor.IgnoreTemperature) {
                return;
            }

            float temperature = 25f; // todo neiran add temperature service
            if (infectionGrowthDescriptor.MinPreferredTemperature < temperature && infectionGrowthDescriptor.MaxPreferredTemperature > temperature) {
                multiplier += infectionGrowthDescriptor.TemperatureGrowBuff;
            }
        }

        private void TryDiseaseGrowToNextStage(DiseaseModel diseaseModel, InfectionStage infectionStage, DiseaseInfectionModel diseaseInfectionModel)
        {
            if (diseaseModel.CurrentGrowth < infectionStage.StageGrowth) {
                return;
            }

            int newPossibleStage = diseaseModel.Stage + 1;
            InfectionStage? newPossibleStageDescriptor = diseaseInfectionModel.InfectionStages.Find(stage => stage.Stage == newPossibleStage);
            if (newPossibleStageDescriptor == null) {
                return;
            }

            diseaseModel.AddSymptoms(infectionStage.RandomSymptoms);
            diseaseModel.Stage = newPossibleStage;
            diseaseModel.CurrentGrowth = 0f;
        }

        private void TryGetInfected(ref PlantModel plant,
                                    PlantsDescriptorModel plantsDescriptorModel,
                                    string soilId,
                                    DiseasesDescriptor diseasesDescriptor)
        {
            foreach ((string id, DiseaseModelDescriptor descriptor) in diseasesDescriptor.Items) {
                if (!descriptor.AffectedPlants.Contains(plantsDescriptorModel.FamilyType)) {
                    continue;
                }

                float infectionChance = CalculateInfectionChance(plant, descriptor.InfectionModel);
                float nearbyMultiplier = CalculateNearbyPlantsMultiplier(plant, id, descriptor.InfectionModel, soilId);

                infectionChance *= nearbyMultiplier;

                float random = Random.Range(0f, 1f);
                if (infectionChance > random) {
                    Debug.Log($"Plant has been infected! PlantId={plant.PlantId}, infectionId={id}, infectionChance={infectionChance}, nearbyMultiplier={nearbyMultiplier}");
                    plant.DiseaseModels.Add(new(id, 1, 0f));
                }
            }
        }

        private float CalculateNearbyPlantsMultiplier(PlantModel plant, string diseaseId, DiseaseInfectionModel diseaseInfectionModel, string soilId)
        {
            float multiplier = 0f;
            int range = diseaseInfectionModel.GetMaxTileRange();

            Dictionary<string, int> nearbyTiles = _worldTileService.GetNearbyTiles(soilId, range);
            foreach ((string tileId, int tileRange) in nearbyTiles) {
                if (!_plantsRepo.Exists(tileId)) {
                    continue;
                }

                PlantModel nearbyPlant = _plantsRepo.Require(tileId);
                if (nearbyPlant.FamilyType != plant.FamilyType) {
                    continue;
                }

                if (nearbyPlant.DiseaseModels.Count == 0) {
                    continue;
                }

                DiseaseModel? plantDisease = nearbyPlant.DiseaseModels.Find(disease => disease.Id == diseaseId);
                if (plantDisease == null) {
                    continue;
                }

                TileRangePair? tileRangePair = diseaseInfectionModel.TileRangeMultipliers.Find(pair => pair.TileRange == tileRange);
                if (tileRangePair == null) {
                    Debug.LogWarning($"Tile range multiplier mismatch. This should not be possible. PlantId={plant.PlantId}, diseaseId={diseaseId}, tileRange={tileRange}");
                    continue;
                }

                multiplier += tileRangePair.Multiplier;
            }

            return Mathf.Max(1f, multiplier);
        }

        private float CalculateInfectionChance(PlantModel plant, DiseaseInfectionModel infectionModel)
        {
            float baseChance = infectionModel.StartingChance;

            if (plant.Immunity > infectionModel.HighImmunity) {
                baseChance *= infectionModel.HighImmunityMultiplier;
            }

            if (plant.Immunity < infectionModel.LowImmunity) {
                baseChance *= infectionModel.LowImmunityMultiplier;
            }

            if (plant.Health > infectionModel.HighHealth) {
                baseChance *= infectionModel.HighImmunityMultiplier;
            }

            if (plant.Health < infectionModel.LowHealth) {
                baseChance *= infectionModel.LowImmunityMultiplier;
            }

            return baseChance;
        }

        public List<SavedDiseaseModel> GetSavedDiseases(PlantModel plantModel)
        {
            DiseasesDescriptor diseasesDescriptor = _descriptorService.Require<DiseasesDescriptor>();

            List<SavedDiseaseModel> savedDiseaseModels = new();

            foreach (DiseaseModel disease in plantModel.DiseaseModels) {
                DiseaseModelDescriptor? diseaseModelDescriptor = diseasesDescriptor.Get(disease.Id);
                if (diseaseModelDescriptor == null) {
                    Debug.LogWarning($"Disease does not exist with id={disease.Id}");
                    continue;
                }

                SavedDiseaseModel newSavedDisease = new(disease.Id, plantModel.FamilyType,
                                                        diseaseModelDescriptor.InfectionModel.DisposeCropRotationNeeded,
                                                        diseaseModelDescriptor.InfectionModel.DisposeDaysNeeded);
                savedDiseaseModels.Add(newSavedDisease);
            }

            return savedDiseaseModels;
        }
    }
}