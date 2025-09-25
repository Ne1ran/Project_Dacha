namespace Game.Calendar.Event
{
    public class TimeChangeEvent
    {
        public const string PASSED = "Passed";
        
        public int Difference { get; }
        public int NewTime { get; }

        public TimeChangeEvent(int difference, int newTime)
        {
            Difference = difference;
            NewTime = newTime;
        }
    }
}