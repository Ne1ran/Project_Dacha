using Game.Plants.Model;

namespace Game.Harvest.PlantHarvest
{
    public interface IPlantHarvestHandler
    {
        void Initialize();
        void GrowHarvest(ref PlantModel plant);
    }
}