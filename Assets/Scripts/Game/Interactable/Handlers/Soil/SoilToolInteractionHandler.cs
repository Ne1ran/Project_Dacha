using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using Game.Tools.Service;
using VContainer;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("UseSoilTool")]
    public class SoilToolInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly ToolsService _toolsService = null!;
        [Inject]
        private readonly NotificationManager _notificationManager = null!;

        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            PieMenuItemSelectionModel pieMenuItemSelectionModel = itemModel.SelectionModels[itemModel.CurrentSelectionIndex];
            if (string.IsNullOrEmpty(pieMenuItemSelectionModel.ItemId)) {
                return _notificationManager.ShowNotification(NotificationType.TOOL_NOT_FOUND);
            }

            return _toolsService.UseToolAsync(pieMenuItemSelectionModel.ItemId, parameters);
        }
    }
}