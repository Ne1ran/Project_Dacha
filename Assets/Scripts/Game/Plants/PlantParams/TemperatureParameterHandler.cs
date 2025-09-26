using Game.Common.Handlers;
using Game.GameMap.Soil.Model;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Stress.Model;
using Game.Temperature.Model;
using Game.Temperature.Service;
using VContainer;

namespace Game.Plants.PlantParams
{
    [Handler("Temperature")]
    public class TemperatureParameterHandler : IPlantParametersHandler
    {
        [Inject]
        private readonly TemperatureService _temperatureService = null!;

        public PlantGrowCalculationModel ApplyParameters(IPlantParameters plantParams, PlantGrowCalculationModel growModel, SoilModel soilModel)
        {
            TemperatureModel currentTemperatureModel = _temperatureService.GetTemperatureModel();

            float maxDayTemperature = currentTemperatureModel.DayTemperature;
            float minNightTemperature = currentTemperatureModel.NightTemperature;
            float averageTemperature = currentTemperatureModel.AverageTemperature;

            if (plantParams.Min > minNightTemperature) {
                float deviation = plantParams.Min - minNightTemperature;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.LowTemperature, plantParams.StressGain * deviation);
            }

            if (plantParams.Max < maxDayTemperature) {
                float deviation = maxDayTemperature - plantParams.Max;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.HighTemperature, plantParams.StressGain * deviation);
            }

            if (plantParams.MinPreferred < averageTemperature && plantParams.MaxPreferred > averageTemperature) {
                growModel.GrowMultiplier += plantParams.GrowBuff;
            }

            return growModel;
        }
    }
}