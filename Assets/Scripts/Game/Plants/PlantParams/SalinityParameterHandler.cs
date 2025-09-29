using Game.Common.Handlers;
using Game.GameMap.Soil.Model;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Stress.Model;

namespace Game.Plants.PlantParams
{
    [Handler("Salinity")]
    public class SalinityParameterHandler : IPlantParametersHandler
    {
        public PlantGrowCalculationModel ApplyParameters(IPlantParameters plantParams, PlantGrowCalculationModel growModel, SoilModel soilModel)
        {
            float soilHumidity = soilModel.Salinity;
            if (plantParams.Min > soilHumidity) {
                float deviation = plantParams.Min - soilHumidity;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.LowSalinity, plantParams.StressGain * deviation);
            }

            if (plantParams.Max < soilHumidity) {
                float deviation = soilHumidity - plantParams.Max;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.HighSalinity, plantParams.StressGain * deviation);
            }

            if (plantParams.MinPreferred < soilHumidity && plantParams.MaxPreferred > soilHumidity) {
                growModel.GrowMultiplier += plantParams.GrowBuff;
            }

            return growModel;
        }
    }
}