namespace Game.Calendar.Model
{
    public class WeatherGenerationModel
    {
        public MonthType StartMonth { get; }
        public int StartDay { get; }
        public MonthType EndMonth { get; }
        public int EndDay { get; }

        public WeatherGenerationModel(MonthType startMonth, int startDay, MonthType endMonth, int endDay)
        {
            StartMonth = startMonth;
            StartDay = startDay;
            EndMonth = endMonth;
            EndDay = endDay;
        }
    }
}