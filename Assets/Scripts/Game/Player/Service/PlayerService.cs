using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Player.Controller;
using JetBrains.Annotations;

namespace Game.Player.Service
{
    [UsedImplicitly]
    public class PlayerService
    {
        private readonly IResourceService _resourceService;

        public PlayerService(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public UniTask<PlayerController> CreatePlayerAsync()
        {
            return _resourceService.LoadObjectAsync<PlayerController>();
        }
    }
}