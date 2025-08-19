namespace Game.TimeMove.Event
{
    public class DayChangedEvent
    {
        public const string DAY_FINISHED = "DayFinished";
        public const string DAY_STARTED = "DayStarted";
        
        public int CurrentDay { get; set; }
        public int DayDifference { get; set; }

        public DayChangedEvent(int currentDay, int dayDifference)
        {
            CurrentDay = currentDay;
            DayDifference = dayDifference;
        }
    }
}