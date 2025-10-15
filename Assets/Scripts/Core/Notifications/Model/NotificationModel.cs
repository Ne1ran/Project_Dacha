namespace Core.Notifications.Model
{
    public class NotificationModel
    {
        public NotificationType Type { get; }
        public string Title { get; }
        public string Message { get; }
        public string? Icon { get; }
        public NotificationAlignment Alignment { get; }
        public float ShowTime { get; }
        public int Priority { get; }

        public NotificationModel(NotificationType type, string message, string title, string? icon, NotificationAlignment alignment, float showTime, int priority)
        {
            Type = type;
            Message = message;
            Title = title;
            Icon = icon;
            Alignment = alignment;
            ShowTime = showTime;
            Priority = priority;
        }
    }
}