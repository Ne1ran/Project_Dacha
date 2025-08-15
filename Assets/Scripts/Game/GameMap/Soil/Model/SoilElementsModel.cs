namespace Game.GameMap.Soil.Model
{
    public class SoilElementsModel
    {
        public float Nitrogen { get; set; }
        public float Potassium { get; set; }
        public float Phosphorus { get; set; }

        public SoilElementsModel(float nitrogen, float potassium, float phosphorus)
        {
            Nitrogen = nitrogen;
            Potassium = potassium;
            Phosphorus = phosphorus;
        }
    }
}