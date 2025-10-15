using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Notifications.Component;
using Core.Notifications.Descriptor;
using Core.Notifications.Model;
using Core.UI.Service;
using Cysharp.Threading.Tasks;

namespace Core.Notifications.Service
{
    [Manager]
    public class NotificationManager : IDisposable
    {
        private readonly UIService _uiService;
        private readonly NotificationsDescriptor _notificationsDescriptor;
        private readonly Dictionary<NotificationModel, NotificationController> _notifications = new();

        public NotificationManager(UIService uiService, NotificationsDescriptor notificationsDescriptor)
        {
            _uiService = uiService;
            _notificationsDescriptor = notificationsDescriptor;
        }

        public void Dispose()
        {
            foreach (NotificationController controller in _notifications.Values) {
                controller.OnNotificationShowed -= OnNotificationShowed;
            }
        }

        public async UniTask ShowNotification(NotificationType notificationType)
        {
            NotificationModel notificationModel = CreateNotificationModel(notificationType);
            NotificationController notification = await _uiService.ShowElementAsync<NotificationController>();
            await notification.InitializeAsync(notificationModel);
            notification.OnNotificationShowed += OnNotificationShowed;
            _notifications.Add(notificationModel, notification);
        }

        private void OnNotificationShowed(NotificationModel notification)
        {
            if (!_notifications.TryGetValue(notification, out NotificationController notificationController)) {
                return;
            }

            _uiService.RemoveElementAsync(notificationController.gameObject).Forget();
            _notifications.Remove(notification);
        }

        private NotificationModel CreateNotificationModel(NotificationType notificationType)
        {
            NotificationModelDescriptor modelDescriptor = _notificationsDescriptor.Require(notificationType);
            return new(notificationType, modelDescriptor.Message, modelDescriptor.Title, modelDescriptor.Icon?.AssetGUID, modelDescriptor.Alignment,
                       modelDescriptor.ShowTimeSeconds, modelDescriptor.Priority);
        }
    }
}