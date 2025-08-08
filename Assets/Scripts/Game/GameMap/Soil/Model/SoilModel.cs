namespace Game.GameMap.Soil.Model
{
    public class SoilModel
    {
        public float Ph { get; set; }
        public int Salinity { get; set; }
        public float Breathability { get; set; }
        public float Humus { get; set; }
        public SoilType Type { get; set; }

        public SoilModel(float ph, int salinity, float breathability, float humus, SoilType type)
        {
            Ph = ph;
            Salinity = salinity;
            Breathability = breathability;
            Humus = humus;
            Type = type;
        }
    }
}