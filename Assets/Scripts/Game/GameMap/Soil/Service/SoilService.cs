using Core.Descriptors.Service;
using Game.GameMap.Soil.Descriptor;
using Game.GameMap.Soil.Model;
using Game.Utils;
using JetBrains.Annotations;

namespace Game.GameMap.Soil.Service
{
    [UsedImplicitly]
    public class SoilService
    {
        private readonly IDescriptorService _descriptorService;

        public SoilService(IDescriptorService descriptorService)
        {
            _descriptorService = descriptorService;
        }

        public SoilModel CreateSoil(SoilType soilType)
        {
            SoilDescriptorModel soilDesc = RequireModelByType(soilType);
            return new(soilDesc.SoilType, soilDesc.Ph, soilDesc.Salinity, soilDesc.Breathability, soilDesc.Humus, soilDesc.Mass);
        }

        public void UpdateSoil()
        {
            // todo neiran do smth we take changes in gramms/kilogramms. Then calculate percents. If its not higher than max values - apply it in percents.
        }

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