using Game.Soil.Component;

namespace Game.Soil.Event
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