using Game.Common.Handlers;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Soil.Model;
using Game.Stress.Model;

namespace Game.Plants.PlantParams
{
    [Handler("SoilHumidity")]
    public class SoilHumidityParameterHandler : IPlantParametersHandler
    {
        public PlantGrowCalculationModel ApplyParameters(IPlantParameters plantParams, PlantGrowCalculationModel growModel, SoilModel soilModel)
        {
            float soilHumidity = soilModel.SoilHumidity;
            if (plantParams.Min > soilHumidity) {
                float deviation = plantParams.Min - soilHumidity;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.LowSoilHumidity, plantParams.StressGain * deviation);
            }

            if (plantParams.Max < soilHumidity) {
                float deviation = soilHumidity - plantParams.Max;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.HighSoilHumidity, plantParams.StressGain * deviation);
            }

            if (plantParams.MinPreferred < soilHumidity && plantParams.MaxPreferred > soilHumidity) {
                growModel.GrowMultiplier += plantParams.GrowBuff;
            }

            return growModel;
        }
    }
}