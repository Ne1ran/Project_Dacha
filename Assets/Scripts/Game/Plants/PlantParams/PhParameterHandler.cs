using Game.Common.Handlers;
using Game.GameMap.Soil.Model;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Stress.Model;

namespace Game.Plants.PlantParams
{
    [Handler("Ph")]
    public class PhParameterHandler : IPlantParametersHandler
    {
        public PlantGrowCalculationModel ApplyParameters(IPlantParameters plantParams, PlantGrowCalculationModel growModel, SoilModel soilModel)
        {
            float currentPh = soilModel.Ph;
            if (plantParams.Min > currentPh) {
                float deviation = currentPh - plantParams.Min;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.LowPh, plantParams.StressGain * deviation);
            }

            if (plantParams.Max < currentPh) {
                float deviation = plantParams.Max - currentPh;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.HighPh, plantParams.StressGain * deviation);
            }

            if (plantParams.MinPreferred < currentPh && plantParams.MaxPreferred > currentPh) {
                growModel.GrowMultiplier += plantParams.GrowBuff;
            }

            return growModel;
        }
    }
}