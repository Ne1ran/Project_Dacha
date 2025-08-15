using Core.Attributes;
using Core.Repository;
using Core.Serialization;
using Game.GameMap.Tiles.Model;

namespace Game.GameMap.Tiles.Repo
{
    [Repository]
    public class TileRepo : SingleEntityPrefsRepository<TilesModel>
    {
        protected override string Key => "Tiles";

        public TileRepo(ISerializer deserializer) : base(deserializer)
        {
        }
    }
}