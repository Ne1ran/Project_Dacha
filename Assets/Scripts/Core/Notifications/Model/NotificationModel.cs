using Core.Notifications.Descriptor;
using UnityEngine;

namespace Core.Notifications.Model
{
    public class NotificationModel
    {
        public string Id { get; }
        public string Title { get; }
        public string Message { get; }
        public Sprite? Icon { get; }
        public NotificationAlignment Alignment { get; }
        public int ShowTime { get; }
        public int Priority { get; }

        public NotificationModel(NotificationModelDescriptor descriptor)
        {
            Id = descriptor.Id;
            Message = descriptor.Message;
            Title = descriptor.Title;
            Icon = descriptor.Icon;
            Alignment = descriptor.Alignment;
            ShowTime = descriptor.ShowTimeSeconds;
            Priority = descriptor.Priority;
        }
    }
}