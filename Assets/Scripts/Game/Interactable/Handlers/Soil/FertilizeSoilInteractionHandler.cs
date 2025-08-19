using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Service;
using Game.PieMenu.Model;
using VContainer;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("FertilizeSoil")]
    public class FertilizeSoilInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly SoilService _soilService = null!;
        [Inject]
        private readonly NotificationManager _notificationManager = null!;

        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            PieMenuItemSelectionModel pieMenuItemSelectionModel = itemModel.SelectionModels[itemModel.CurrentSelectionIndex];
            if (string.IsNullOrEmpty(pieMenuItemSelectionModel.ItemId)) {
                return _notificationManager.ShowNotification(NotificationType.FERTILIZER_NOT_FOUND);
            }
            
            string tileId = parameters.Require<string>(ParameterNames.TileId);
            string fertilizerId = pieMenuItemSelectionModel.ItemId;
            float portionMass = 100f; // todo remove afterwards
            UseFertilizer(tileId, fertilizerId, portionMass);
            return UniTask.CompletedTask;
        }

        private void UseFertilizer(string tileId, string fertilizerId, float portionMass)
        {
            _soilService.AddFertilizer(tileId, fertilizerId, portionMass);
        }
    }
}