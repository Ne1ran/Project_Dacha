using Game.Soil.Model;

namespace Game.GameMap.Tiles.Event
{
    public class SoilUpdatedEvent
    {
        public const string Updated = "FullyUpdated";
        
        public string Id { get; }
        public SoilModel SoilModel { get; }

        public SoilUpdatedEvent(string id, SoilModel soilModel)
        {
            Id = id;
            SoilModel = soilModel;
        }
    }
}