using Core.Notifications.Model;
using Core.Notifications.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Service;
using VContainer;

namespace Game.Tools.Handlers
{
    [Handler("Bucket")]
    public class UseBucketHandler : IUseToolHandler
    {
        [Inject]
        private readonly SoilService _soilService = null!;
        
        [Inject]
        private readonly NotificationManager _notificationManager = null!;

        public UniTask UseAsync(Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId);
            float waterAmount = parameters.Require<float>(ParameterNames.WaterAmount);
            if (waterAmount <= 0) {
                return _notificationManager.ShowNotification(NotificationType.NO_WATER);
            }
            
            _soilService.WaterSoil(tileId, waterAmount);
            return UniTask.CompletedTask;
        }
    }
}