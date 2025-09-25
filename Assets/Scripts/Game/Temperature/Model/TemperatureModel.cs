namespace Game.Temperature.Model
{
    public class TemperatureModel
    {
        public float AverageTemperature { get; }
        public float DayTemperature { get; }
        public float NightTemperature { get; }

        public TemperatureModel(float averageTemperature, float dayTemperature, float nightTemperature)
        {
            AverageTemperature = averageTemperature;
            DayTemperature = dayTemperature;
            NightTemperature = nightTemperature;
        }
    }
}