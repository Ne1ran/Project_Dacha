using System.Collections.Generic;
using System.Linq;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Tiles.Component;
using Game.GameMap.Tiles.Model;
using JetBrains.Annotations;

namespace Game.GameMap.Tiles.Service
{
    [UsedImplicitly]
    public class WorldTileService
    {
        private readonly IResourceService _resourceService;

        public WorldTileService(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public async UniTask<List<TileController>> CreateTilesInWorldAsync(List<SingleTileModel> tiles)
        {
            List<UniTask<TileController>> tasks = new();

            foreach (SingleTileModel singleTileModel in tiles) {
                tasks.Add(CreateTileAsync(singleTileModel));
            }

            List<TileController> tileControllers = (await UniTask.WhenAll(tasks)).ToList();
            return tileControllers;
        }

        private async UniTask<TileController> CreateTileAsync(SingleTileModel tileModel)
        {
            TileController tileController = await _resourceService.LoadObjectAsync<TileController>();
            tileController.Initialize(tileModel);
            tileController.transform.position = tileModel.Position.ToVector3();
            return tileController;
        }
    }
}