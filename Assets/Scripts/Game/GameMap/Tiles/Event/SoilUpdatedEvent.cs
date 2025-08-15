using Game.GameMap.Soil.Model;

namespace Game.GameMap.Tiles.Event
{
    public class SoilUpdatedEvent
    {
        public const string SOIL_UPDATED = "SOIL_UPDATED";
        
        public string TileId { get; }
        public SoilModel SoilModel { get; }

        public SoilUpdatedEvent(string tileId, SoilModel soilModel)
        {
            TileId = tileId;
            SoilModel = soilModel;
        }
    }
}