using Game.Plants.Component;

namespace Game.Plants.Event
{
    public class PlantControllerCreatedEvent
    {
        public const string PlantCreated = "PlantControllerCreated";
        
        public string TileId { get; }
        public PlantController PlantController { get; }

        public PlantControllerCreatedEvent(string tileId, PlantController plantController)
        {
            TileId = tileId;
            PlantController = plantController;
        }
    }
}