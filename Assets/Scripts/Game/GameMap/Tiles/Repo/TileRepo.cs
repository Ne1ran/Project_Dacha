using Core.Attributes;
using Core.Repository;
using Game.GameMap.Tiles.Model;

namespace Game.GameMap.Tiles.Repo
{
    [Repository]
    public class TileRepo : SingleEntityMemoryRepository<TilesModel>
    {
   
    }
}