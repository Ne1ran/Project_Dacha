using Game.GameMap.Soil.Component;

namespace Game.GameMap.Soil.Event
{
    public class SoilControllerCreatedEvent
    {
        public const string SoilCreated = "SoilControllerCreated";
        
        public string Id { get; }
        public SoilController SoilController { get; }

        public SoilControllerCreatedEvent(string id, SoilController soilController)
        {
            Id = id;
            SoilController = soilController;
        }
    }
}