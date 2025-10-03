using Game.Common.Handlers;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Soil.Model;
using Game.Stress.Model;
using Game.Sunlight.Service;
using UnityEngine;
using VContainer;

namespace Game.Plants.PlantParams
{
    [Handler("Sunlight")]
    public class SunlightParametersHandler : IPlantParametersHandler
    {
        [Inject]
        private readonly SunlightService _sunlightService = null!;
        
        public PlantGrowCalculationModel ApplyParameters(IPlantParameters plantParams, PlantGrowCalculationModel growModel, SoilModel soilModel)
        {
            float currentSunlight = _sunlightService.GetDailySunAmount();
            Debug.LogWarning($"Current sunlight for plant is = {currentSunlight}");
            if (plantParams.Min > currentSunlight) {
                float deviation = currentSunlight - plantParams.Min;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.LowSunlight, plantParams.StressGain * deviation);
            }

            if (plantParams.Max < currentSunlight) {
                float deviation = plantParams.Max - currentSunlight;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.HighSunlight, plantParams.StressGain * deviation);
            }

            if (plantParams.MinPreferred < currentSunlight && plantParams.MaxPreferred > currentSunlight) {
                growModel.GrowMultiplier += plantParams.GrowBuff;
            }

            return growModel;
        }
    }
}