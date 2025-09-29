using Game.GameMap.Soil.Model;
using Game.Plants.Descriptors;
using Game.Plants.Model;

namespace Game.Plants.PlantParams
{
    public interface IPlantParametersHandler
    {
        public PlantGrowCalculationModel ApplyParameters(IPlantParameters plantParams, PlantGrowCalculationModel growModel, SoilModel soilModel);
    }
}