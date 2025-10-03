namespace Game.Weather.Model
{
    public class WeatherModel
    {
        public WeatherType WeatherType { get; }
        public float Precipitations { get; }
        public WindType WindType { get; }

        public WeatherModel(WeatherType weatherType, float precipitations, WindType windType)
        {
            WeatherType = weatherType;
            Precipitations = precipitations;
            WindType = windType;
        }
    }
}