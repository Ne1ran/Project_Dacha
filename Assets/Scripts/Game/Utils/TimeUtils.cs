namespace Game.Utils
{
    public static class TimeUtils
    {
        private const int SecondsInMinute = 60;
        private const int MinutesInHour = 60;
        private const int HoursInDay = 24;
        private const int DaysInYear = 365;

        public static int MinutesToSeconds(int minutes)
        {
            return minutes * SecondsInMinute;
        }

        public static int HoursToSeconds(int hours)
        {
            return MinutesToSeconds(hours * MinutesInHour);
        }

        public static int DaysToSeconds(int days)
        {
            return HoursToSeconds(days * HoursInDay);
        }

        public static int YearsToSeconds(int years)
        {
            return DaysToSeconds(years * DaysInYear);
        }
    }
}