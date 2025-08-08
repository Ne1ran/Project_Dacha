using Core.Repository;
using Core.Serialization;
using Game.GameMap.Tiles.Model;
using JetBrains.Annotations;

namespace Game.GameMap.Tiles.Repo
{
    [UsedImplicitly]
    public class TileRepo : SingleEntityPrefsRepository<TilesModel>
    {
        protected override string Key => "Tiles";

        public TileRepo(ISerializer deserializer) : base(deserializer)
        {
        }
    }
}