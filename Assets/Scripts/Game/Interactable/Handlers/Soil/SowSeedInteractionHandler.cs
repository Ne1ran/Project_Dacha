using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using Game.Seeds.Service;
using VContainer;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("SowSeed")]
    public class SowSeedSoilInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly SeedsService _seedsService = null!;
        [Inject]
        private readonly NotificationManager _notificationManager = null!;

        public async UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            PieMenuItemSelectionModel pieMenuItemSelectionModel = itemModel.SelectionModels[itemModel.CurrentSelectionIndex];
            if (string.IsNullOrEmpty(pieMenuItemSelectionModel.ItemId)) {
                await _notificationManager.ShowNotification(NotificationType.SeedsNotFound);
                return;
            }

            await _seedsService.SowSeedAsync(pieMenuItemSelectionModel.ItemId, parameters);
        }
    }
}