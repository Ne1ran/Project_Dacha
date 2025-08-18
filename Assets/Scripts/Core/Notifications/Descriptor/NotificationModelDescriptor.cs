using System;
using Core.Notifications.Model;
using UnityEngine;

namespace Core.Notifications.Descriptor
{
    [Serializable]
    public class NotificationModelDescriptor
    {
        [field: SerializeField]
        public string Id { get; set; } = string.Empty;
        [field: SerializeField]
        public NotificationType Type { get; set; }
        [field: SerializeField]
        public string Title { get; set; } = null!;
        [field: SerializeField]
        public string Message { get; set; } = null!;
        [field: SerializeField]
        public Sprite? Icon { get; set; }
        [field: SerializeField]
        public NotificationAlignment Alignment { get; set; } = NotificationAlignment.LOWER_RIGHT;
        [field: SerializeField]
        public int ShowTimeSeconds { get; set; } = 2;
        [field: SerializeField]
        public int Priority { get; set; } = 100;
    }
}