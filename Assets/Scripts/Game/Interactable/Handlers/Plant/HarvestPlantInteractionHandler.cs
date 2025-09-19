using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Harvest.Service;
using Game.Inventory.Service;
using Game.PieMenu.Model;
using VContainer;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("HarvestPlant")]
    public class HarvestPlantInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly InventoryService _inventoryService = null!;
        [Inject]
        private readonly PlantHarvestService _plantHarvestService = null!;
        [Inject]
        private readonly NotificationManager _notificationManager = null!;

        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            if (_inventoryService.CheckIsFull()) {
                _notificationManager.ShowNotification(NotificationType.InventoryFull).Forget();
                return UniTask.CompletedTask;
            }

            string tileId = parameters.Require<string>(ParameterNames.TileId);
            if (!_plantHarvestService.TryHarvestPlant(tileId)) {
                _notificationManager.ShowNotification(NotificationType.CannotHarvestPlant).Forget();
            }
            
            return UniTask.CompletedTask;
        }
    }
}