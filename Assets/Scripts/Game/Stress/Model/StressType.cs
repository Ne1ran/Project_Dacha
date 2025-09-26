namespace Game.Stress.Model
{
    public enum StressType
    {
        None = 0,
        LowTemperature = 10,
        HighTemperature = 11,
        LowSalinity = 20,
        HighSalinity = 21,
        LowSoilHumidity = 30,
        HighSoilHumidity = 31,
        LowAirHumidity = 40,
        HighAirHumidity = 41,
        ConsumptionOverall = 50,
        ConsumptionNitrogen = 51,
        ConsumptionPotassium = 52,
        ConsumptionPhosphorus = 53,
        LowSunlight = 60,
        HighSunlight = 61,
        LowPh = 70,
        HighPh = 71,
        Decease = 80,
        Other = 100,
    }
}