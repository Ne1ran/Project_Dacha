using Game.Common.Handlers;
using Game.Humidity.Service;
using Game.Plants.Descriptors;
using Game.Plants.Model;
using Game.Soil.Model;
using Game.Stress.Model;
using UnityEngine;
using VContainer;

namespace Game.Plants.PlantParams
{
    [Handler("AirHumidity")]
    public class AirHumidityParameterHandler : IPlantParametersHandler
    {
        [Inject]
        private readonly AirHumidityService _airHumidityService = null!;

        public PlantGrowCalculationModel ApplyParameters(IPlantParameters plantParams, PlantGrowCalculationModel growModel, SoilModel soilModel)
        {
            float airHumidityPercent = _airHumidityService.GetDailyAirHumidityPercent();

            Debug.Log($"Air humidity affect is = {airHumidityPercent}");

            if (plantParams.Min > airHumidityPercent) {
                float deviation = plantParams.Min - airHumidityPercent;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.LowAirHumidity, plantParams.StressGain * deviation);
            }

            if (plantParams.Max < airHumidityPercent) {
                float deviation = airHumidityPercent - plantParams.Max;
                growModel.Damage += plantParams.DamagePerDeviation * deviation;
                growModel.Stress.TryAdd(StressType.HighAirHumidity, plantParams.StressGain * deviation);
            }

            if (plantParams.MinPreferred < airHumidityPercent && plantParams.MaxPreferred > airHumidityPercent) {
                growModel.GrowMultiplier += plantParams.GrowBuff;
            }

            return growModel;
        }
    }
}