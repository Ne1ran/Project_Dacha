using System.Collections.Generic;
using Core.Attributes;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Notifications.Descriptor
{
    [CreateAssetMenu(fileName = "NotificationsDescriptor", menuName = "Dacha/Descriptors/NotificationsDescriptor")]
    [Descriptor("Descriptors/" + nameof(NotificationsDescriptor))]
    public class NotificationsDescriptor : ScriptableObject
    {
        [field: SerializeField]
        [TableList]
        public List<NotificationModelDescriptor> Items { get; private set; } = new();

        private void OnValidate()
        {
            foreach (NotificationModelDescriptor item in Items) {
                if (string.IsNullOrEmpty(item.Id) && !string.IsNullOrEmpty(item.Title)) {
                    item.Id = item.Type.ToUpperFirstString();
                }
            }
        }
    }
}