using Core.Attributes;
using Core.Descriptors.Descriptor;
using Core.Notifications.Model;
using UnityEngine;

namespace Core.Notifications.Descriptor
{
    [CreateAssetMenu(fileName = "NotificationsDescriptor", menuName = "Dacha/Descriptors/NotificationsDescriptor")]
    [Descriptor("Descriptors/" + nameof(NotificationsDescriptor))]
    public class NotificationsDescriptor : Descriptor<NotificationType, NotificationModelDescriptor>
    {
        
    }
}