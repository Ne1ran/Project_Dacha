using Core.Attributes;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Player.Controller;

namespace Game.Player.Service
{
    [Service]
    public class PlayerService
    {
        private readonly IResourceService _resourceService;

        public PlayerController Player { get; private set; } = null!;

        public PlayerService(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public async UniTask<PlayerController> CreatePlayerAsync()
        {
            PlayerController playerController = await _resourceService.LoadObjectAsync<PlayerController>();
            Player = playerController;
            return playerController;
        }
    }
}