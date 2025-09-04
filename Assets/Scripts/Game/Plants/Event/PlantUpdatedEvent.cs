using Game.Plants.Model;

namespace Game.Plants.Event
{
    public class PlantUpdatedEvent
    {
        public const string Created = "PlantCreated";
        public const string Updated = "PlantUpdated";
        public const string Removed = "PlantRemoved";
        
        public string TileId { get; }
        public PlantModel PlantModel { get; }

        public PlantUpdatedEvent(string tileId, PlantModel plantModel)
        {
            TileId = tileId;
            PlantModel = plantModel;
        }
    }
}