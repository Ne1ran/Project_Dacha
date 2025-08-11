namespace Game.GameMap.Soil.Model
{
    public class SoilModel
    {
        public SoilType Type { get; set; }
        public float Ph { get; set; }
        public float Salinity { get; set; }
        public float Breathability { get; set; }
        public float Humus { get; set; }
        public float Mass { get; set; }

        public SoilModel(SoilType type, float ph, float salinity, float breathability, float humus, float mass)
        {
            Type = type;
            Ph = ph;
            Salinity = salinity;
            Breathability = breathability;
            Humus = humus;
            Mass = mass;
        }
    }
}