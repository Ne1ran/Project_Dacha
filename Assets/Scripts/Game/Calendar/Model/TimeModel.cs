namespace Game.Calendar.Model
{
    public class TimeModel
    {
        public int CurrentMinutes { get; set; }
        public int CurrentDay { get; set; }
        public int CurrentMonth { get; set; }
        public int CurrentYear { get; set; }

        public TimeModel(int currentMinutes, int currentDay, int currentMonth, int currentYear)
        {
            CurrentMinutes = currentMinutes;
            CurrentDay = currentDay;
            CurrentMonth = currentMonth;
            CurrentYear = currentYear;
        }
    }
}