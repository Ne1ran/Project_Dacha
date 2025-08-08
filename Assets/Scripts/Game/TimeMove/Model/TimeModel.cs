namespace Game.TimeMove.Model
{
    public class TimeModel
    {
        public int CurrentTime { get; set; }

        public TimeModel(int currentTime)
        {
            CurrentTime = currentTime;
        }
    }
}