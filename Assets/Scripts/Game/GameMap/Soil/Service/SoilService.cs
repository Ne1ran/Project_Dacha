using System;
using Core.Attributes;
using Core.Descriptors.Service;
using Game.Fertilizers.Descriptor;
using Game.Fertilizers.Model;
using Game.GameMap.Soil.Descriptor;
using Game.GameMap.Soil.Model;
using Game.GameMap.Tiles.Event;
using Game.Utils;
using MessagePipe;

namespace Game.GameMap.Soil.Service
{
    [Service]
    public class SoilService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly IPublisher<string, SoilUpdatedEvent> _soilUpdatedPublisher;

        public SoilService(IDescriptorService descriptorService, IPublisher<string, SoilUpdatedEvent> soilUpdatedPublisher)
        {
            _descriptorService = descriptorService;
            _soilUpdatedPublisher = soilUpdatedPublisher;
        }

        public SoilModel CreateSoil(SoilType soilType)
        {
            SoilDescriptorModel soilDesc = RequireModelByType(soilType);
            SoilElementsDescriptorModel elementsDescriptor = soilDesc.ElementsDescriptorModel;
            SoilElementsModel soilElementsModel = new(elementsDescriptor.StartNitrogen,
                                                      elementsDescriptor.StartPotassium, elementsDescriptor.StartPhosphorus);

            return new(soilDesc.SoilType, soilDesc.Ph, soilDesc.Salinity, soilDesc.Breathability, soilDesc.Humus * soilDesc.Mass, soilDesc.Mass,
                       soilDesc.StartWaterAmount, soilElementsModel);
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

        // Use from tile service
        public SoilModel TryRecoverSoil(SoilModel soil, int daysPassed)
        {
            SoilDescriptorModel soilDesc = RequireModelByType(soil.Type);
            if (!NeedRecover(soil, soilDesc)) {
                return soil;
            }

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

        private bool NeedRecover(SoilModel soil, SoilDescriptorModel soilDesc)
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
    }
}