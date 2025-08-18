using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using VContainer;

namespace Game.PieMenu.ActionHandler
{
    [Handler("FertilizerNotFound")]
    public class FertilizerNotFoundActionHandler : IPieMenuActionHandler
    {
        [Inject]
        private readonly NotificationManager _notificationManager = null!;

        private NotificationType _notificationType;

        public void Initialize(Parameters parameters)
        {
            _notificationType = parameters.Require<NotificationType>(ParameterNames.NotificationType);
        }

        public UniTask ActionAsync()
        {
            return _notificationManager.ShowNotification(_notificationType);
        }
    }
}