using Game.GameMap.Soil.Component;
using Game.GameMap.Tiles.Model;

namespace Game.GameMap.Soil.Event
{
    public class SoilControllerCreatedEvent
    {
        public const string SoilCreated = "SoilControllerCreated";
        
        public SingleTileModel TileModel { get; }
        public SoilController SoilController { get; }

        public SoilControllerCreatedEvent(SingleTileModel tileModel, SoilController soilController)
        {
            TileModel = tileModel;
            SoilController = soilController;
        }
    }
}