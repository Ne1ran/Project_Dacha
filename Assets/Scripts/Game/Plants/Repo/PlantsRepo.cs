using Core.Attributes;
using Core.Repository;
using Game.Plants.Model;

namespace Game.Plants.Repo
{
    [Repository]
    public class PlantsRepo : MemoryRepository<string, PlantModel>
    {
        
    }
}