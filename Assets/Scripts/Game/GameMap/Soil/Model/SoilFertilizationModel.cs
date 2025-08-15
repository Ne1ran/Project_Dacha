namespace Game.GameMap.Soil.Model
{
    public class SoilFertilizationModel
    {
        public string FertilizerId { get; set; }
        public float Mass { get; set; }
        public int CurrentDecomposeDay { get; set; }

        public SoilFertilizationModel(string fertilizerId, float mass, int currentDecomposeDay)
        {
            FertilizerId = fertilizerId;
            Mass = mass;
            CurrentDecomposeDay = currentDecomposeDay;
        }
    }
}