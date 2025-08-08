namespace Game.Soil.Model
{
    public class SoilModel
    {
        public float Ph { get; set; }
        public int Salinity { get; set; }
        public float Breathability { get; set; }
        public float Humus { get; set; }
        public SoilType Type { get; set; }
    }
}