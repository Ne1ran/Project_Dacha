using System;
using Core.Notifications.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Notifications.Descriptor
{
    [Serializable]
    public class NotificationModelDescriptor
    {
        [field: SerializeField]
        public string Title { get; set; } = null!;
        [field: SerializeField]
        public string Message { get; set; } = null!;
        [field: SerializeField]
        public AssetReference? Icon { get; set; }
        [field: SerializeField]
        public NotificationAlignment Alignment { get; set; } = NotificationAlignment.LOWER_RIGHT;
        [field: SerializeField]
        public float ShowTimeSeconds { get; set; } = 2f;
        [field: SerializeField]
        public int Priority { get; set; } = 100;
    }
}