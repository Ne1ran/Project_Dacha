using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Diseases.Model;
using Game.Fertilizers.Descriptor;
using Game.Fertilizers.Model;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Soil.Descriptor;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Repository;
using Game.GameMap.Tiles.Event;
using Game.Plants.Model;
using Game.TimeMove.Event;
using Game.Utils;
using MessagePipe;
using UnityEngine;

namespace Game.GameMap.Soil.Service
{
    [Service]
    public class SoilService : IDisposable
    {
        private readonly SoilRepo _soilRepo;
        private readonly IDescriptorService _descriptorService;
        private readonly IPublisher<string, SoilUpdatedEvent> _soilUpdatedPublisher;
        private IDisposable? _disposable;

        public SoilService(IDescriptorService descriptorService,
                           IPublisher<string, SoilUpdatedEvent> soilUpdatedPublisher,
                           ISubscriber<string, DayChangedEvent> dayFinishedSubscriber,
                           SoilRepo soilRepo)
        {
            _descriptorService = descriptorService;
            _soilUpdatedPublisher = soilUpdatedPublisher;
            _soilRepo = soilRepo;

            DisposableBagBuilder bagBuilder = DisposableBag.CreateBuilder();
            bagBuilder.Add(dayFinishedSubscriber.Subscribe(DayChangedEvent.DAY_FINISHED, OnDayFinished));
            bagBuilder.Add(dayFinishedSubscriber.Subscribe(DayChangedEvent.DAY_STARTED, OnDayStarted));
            _disposable = bagBuilder.Build();
        }

        public void InitializeFromSave()
        {
            // todo neiran implement with save system
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public SoilModel CreateSoil()
        {
            SoilType mapSoilType = _descriptorService.Require<MapDescriptor>().SoilType;
            SoilDescriptorModel soilDesc = RequireModelByType(mapSoilType);
            SoilElementsDescriptorModel elementsDescriptor = soilDesc.ElementsDescriptorModel;
            SoilElementsModel soilElementsModel = new(elementsDescriptor.StartNitrogen,
                                                      elementsDescriptor.StartPotassium, elementsDescriptor.StartPhosphorus);

            return new(soilDesc.SoilType, soilDesc.Ph, soilDesc.Salinity, soilDesc.Breathability, soilDesc.Humus * soilDesc.Mass, soilDesc.Mass,
                       soilDesc.StartWaterAmount, soilElementsModel);
        }

        public void AddSavedDisease(string tileId, SavedDiseaseModel diseaseModel)
        {
            GerOrCreate(tileId).SavedDiseases.Add(diseaseModel);
        }

        public void ShovelSoil(string tileId)
        {
            
        }

        public SoilModel ActivateUsedFertilizers(SoilModel soilModel)
        {
            if (soilModel.UsedFertilizers.Count == 0) {
                return soilModel;
            }

            FertilizersDescriptor fertilizersDescriptor = _descriptorService.Require<FertilizersDescriptor>();

            for (int i = 0; i < soilModel.UsedFertilizers.Count; i++) {
                SoilFertilizationModel usedFertilizer = soilModel.UsedFertilizers[i];
                if (usedFertilizer.CurrentDecomposeDay == 0) {
                    continue;
                }

                FertilizerDescriptorModel? fertModel = fertilizersDescriptor.Fertilizers.Find(fert => fert.Id == usedFertilizer.FertilizerId);
                if (fertModel == null) {
                    throw new ArgumentException($"Fertilizer not found with id={usedFertilizer.FertilizerId}");
                }

                FertilizerSoilModel fertilizerSoilModel = CalculateSoilFertilizerModel(usedFertilizer, fertModel);
                soilModel.ApplyFertilizer(fertilizerSoilModel);
                if (usedFertilizer.CurrentDecomposeDay >= fertModel.DecomposeTime) {
                    soilModel.UsedFertilizers.Remove(usedFertilizer);
                }
            }

            return soilModel;
        }

        private FertilizerSoilModel CalculateSoilFertilizerModel(SoilFertilizationModel usedFertilizer, FertilizerDescriptorModel fertModel)
        {
            float usedMassPercentage = usedFertilizer.Mass / fertModel.StartMass;
            float decomposeDayPercent = (float) usedFertilizer.CurrentDecomposeDay / fertModel.DecomposeTime;
            float decomposedMass = usedFertilizer.Mass * decomposeDayPercent;

            float massDecompositionPercentage = usedMassPercentage * decomposeDayPercent;
            float phChange = fertModel.PhChange * massDecompositionPercentage;
            float breathabilityValue = fertModel.BreathabilityValue * massDecompositionPercentage;
            float humusValue = fertModel.HumusValue * massDecompositionPercentage;

            FertilizerElementsDescriptorModel elementsDescriptor = fertModel.FertilizerElements;
            float nitrogen = elementsDescriptor.NitrogenPercent * decomposedMass;
            float potassium = elementsDescriptor.PotassiumPercent * decomposedMass;
            float phosphorus = elementsDescriptor.PhosphorusPercent * decomposedMass;

            return new(decomposedMass, phChange, breathabilityValue, humusValue, nitrogen, potassium, phosphorus);
        }

        private void OnDayFinished(DayChangedEvent evt)
        {
            Dictionary<string, SoilModel> soilModels = _soilRepo.GetAll();

            foreach (SoilModel soilModel in soilModels.Values) {
                foreach (SoilFertilizationModel usedFertilizer in soilModel.UsedFertilizers) {
                    usedFertilizer.CurrentDecomposeDay += 1;
                }

                soilModel.DugRecently = false;
                TryRecoverSoil(soilModel, evt.DayDifference);
            }

            _soilRepo.SaveAll(soilModels);
        }

        private void OnDayStarted(DayChangedEvent evt)
        {
            ActivateFertilizers();
        }

        private void ActivateFertilizers()
        {
            Dictionary<string, SoilModel> soilModels = _soilRepo.GetAll();
            foreach (SoilModel soilModel in soilModels.Values) {
                ActivateUsedFertilizers(soilModel);
            }

            _soilRepo.SaveAll(soilModels);
        }

        public void AddFertilizer(string tileId, string fertilizerId, float portionMassGramms)
        {
            SoilModel soilModel = GerOrCreate(tileId);
            AddFertilizer(soilModel, fertilizerId, portionMassGramms / 1000f);
        }

        public void UpdateCropRotation(string tileId, PlantFamilyType plantFamilyType)
        {
            SoilModel soilModel = GerOrCreate(tileId);

            int currentRotation = soilModel.CropRotations.Count;
            int newRotation = currentRotation + 1;
            soilModel.CropRotations.Add(newRotation, plantFamilyType);
            for (int i = 0; i < soilModel.SavedDiseases.Count; i++) {
                SavedDiseaseModel savedDisease = soilModel.SavedDiseases[i];
                if (savedDisease.PlantFamilyType == plantFamilyType) {
                    // todo neiran maybe redo to check not only for last(new) rotation, but multiple?
                    continue;
                }

                savedDisease.CropRotationNeeded--;
                if (savedDisease.CropRotationNeeded <= 0) {
                    soilModel.SavedDiseases.RemoveAt(i);
                }
            }
        }

        public SoilModel AddFertilizer(SoilModel soilModel, string fertilizerId, float mass)
        {
            foreach (SoilFertilizationModel usedFertilizer in soilModel.UsedFertilizers) {
                if (usedFertilizer.FertilizerId != fertilizerId || usedFertilizer.CurrentDecomposeDay != 0) {
                    continue;
                }

                usedFertilizer.Mass += mass;
                return soilModel;
            }

            soilModel.UsedFertilizers.Add(new(fertilizerId, mass, 0));
            return soilModel;
        }

        public SoilModel TryRecoverSoil(SoilModel soil, int daysPassed)
        {
            RecoverFromDiseases(soil, daysPassed);

            SoilDescriptorModel soilDesc = RequireModelByType(soil.Type);
            return NeedRecoverBaseParams(soil, soilDesc) ? RecoverBaseSoilParams(soil, soilDesc, daysPassed) : soil;
        }

        private void RecoverFromDiseases(SoilModel soil, int daysPassed)
        {
            for (int i = 0; i < soil.SavedDiseases.Count; i++) {
                SavedDiseaseModel diseaseModel = soil.SavedDiseases[i];
                diseaseModel.RemoveDaysNeeded -= daysPassed;
                if (diseaseModel.RemoveDaysNeeded <= 0) {
                    soil.SavedDiseases.RemoveAt(i);
                }
            }
        }

        private SoilModel RecoverBaseSoilParams(SoilModel soil, SoilDescriptorModel soilDesc, int daysPassed)
        {
            float phDiff = soilDesc.Ph - soil.Ph;
            float salinityDiff = soilDesc.Salinity - soil.Salinity;
            float breathabilityDiff = soilDesc.Breathability - soil.Breathability;
            float humusDiff = soilDesc.Humus - soil.Humus;

            float dayCoeff = (float) daysPassed / soilDesc.RecoveryDays;

            soil.Ph = phDiff / dayCoeff;
            soil.Salinity = salinityDiff / dayCoeff;
            soil.Breathability = breathabilityDiff / dayCoeff;
            soil.Humus = humusDiff / dayCoeff;
            return soil;
        }

        private bool NeedRecoverBaseParams(SoilModel soil, SoilDescriptorModel soilDesc)
        {
            if (!MathUtils.IsFuzzyEquals(soilDesc.Ph, soil.Ph, 0.01f)) {
                return true;
            }

            if (!MathUtils.IsFuzzyEquals(soilDesc.Salinity, soil.Salinity, 0.01f)) {
                return true;
            }

            if (!MathUtils.IsFuzzyEquals(soilDesc.Breathability, soil.Breathability, 0.1f)) {
                return true;
            }

            if (!MathUtils.IsFuzzyEquals(soilDesc.Humus, soil.Humus, 0.01f)) {
                return true;
            }

            return false;
        }

        private SoilDescriptorModel RequireModelByType(SoilType soilType)
        {
            return _descriptorService.Require<SoilDescriptor>().RequireByType(soilType);
        }

        public SoilModel GerOrCreate(string key)
        {
            if (_soilRepo.Exists(key)) {
                return _soilRepo.Require(key);
            }

            SoilModel soilModel = CreateSoil();
            _soilRepo.Save(key, soilModel);
            return soilModel;
        }
    }
}