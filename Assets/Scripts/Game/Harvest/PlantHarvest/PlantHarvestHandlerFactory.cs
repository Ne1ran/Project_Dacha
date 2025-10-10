using Core.Attributes;
using Game.Common.Handlers;

namespace Game.Harvest.PlantHarvest
{
    [Factory]
    public class PlantHarvestHandlerFactory : HandlerFactory<IPlantHarvestHandler>
    {
    }
}