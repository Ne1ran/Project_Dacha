using Core.Attributes;
using Core.Repository;
using Game.Soil.Model;

namespace Game.Soil.Repository
{
    [Repository]
    public class SoilRepo : MemoryRepository<string, SoilModel>
    {
        
    }
}