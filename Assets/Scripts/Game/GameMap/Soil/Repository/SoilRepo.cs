using Core.Attributes;
using Core.Repository;
using Game.GameMap.Soil.Model;

namespace Game.GameMap.Soil.Repository
{
    [Repository]
    public class SoilRepo : MemoryRepository<string, SoilModel>
    {
        
    }
}