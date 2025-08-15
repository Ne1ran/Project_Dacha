namespace Game.TimeMove.Model
{
    public class TimeModel
    {
        public int CurrentMinutes { get; set; }
        public int CurrentDay { get; set; }

        public TimeModel(int currentMinutes, int currentDay)
        {
            CurrentMinutes = currentMinutes;
            CurrentDay = currentDay;
        }
    }
}