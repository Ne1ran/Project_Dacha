using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Common.Model;
using Core.Descriptors.Service;
using Core.GameWorld.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Tiles.Component;
using Game.GameMap.Tiles.Model;
using Unity.Mathematics.Geometry;
using Math = System.Math;

namespace Game.GameMap.Tiles.Service
{
    [Service]
    public class WorldTileService
    {
        private readonly IResourceService _resourceService;
        private readonly GameWorldService _gameWorldService;
        private readonly IDescriptorService _descriptorService;

        private readonly List<SingleTileModel> _mapTilesModels = new();
        private readonly Dictionary<SingleTileModel, TileController> _mapTilesControllers = new();

        public WorldTileService(IResourceService resourceService, GameWorldService gameWorldService, IDescriptorService descriptorService)
        {
            _resourceService = resourceService;
            _gameWorldService = gameWorldService;
            _descriptorService = descriptorService;
            // todo neiran subscribe on tile/soil/everything changes to change soil visualization
        }

        public async UniTask CreateTilesInWorldAsync(List<SingleTileModel> tiles)
        {
            List<UniTask<TileController>> tasks = new();

            foreach (SingleTileModel singleTileModel in tiles) {
                _mapTilesModels.Add(singleTileModel);
                tasks.Add(CreateTileAsync(singleTileModel));
            }

            await UniTask.WhenAll(tasks);
        }

        public Dictionary<int, string> GetNearbyTiles(string centerTileId, int range)
        {
            Dictionary<int, string> result = new();

            SingleTileModel centerTileModel = _mapTilesModels.Find(tile => tile.Id == centerTileId);
            MapDescriptor mapDescriptor = _descriptorService.Require<MapDescriptor>();
            int centerTileIndex = _mapTilesModels.IndexOf(centerTileModel);
            int length = mapDescriptor.Length;
            int width = mapDescriptor.Width;
            int centerX = centerTileIndex % width;
            int centerY = centerTileIndex / width;
            for (int dy = -range; dy <= range; dy++) {
                for (int dx = -range; dx <= range; dx++) {
                    if (dx == 0 && dy == 0) {
                        continue;
                    }

                    int nx = centerX + dx;
                    int ny = centerY + dy;

                    if (nx < 0 || nx >= width || ny < 0 || ny >= length) {
                        continue;
                    }

                    int neighborIndex = ny * width + nx;
                    int minRange = Math.Min(Math.Abs(dx), Math.Abs(dy));
                    if (minRange == 0) {
                        minRange = 1;
                    }

                    result.Add(minRange, _mapTilesModels[neighborIndex].Id);
                }
            }

            return result;
        }

        private async UniTask<TileController> CreateTileAsync(SingleTileModel tileModel)
        {
            TileController tileController = await _resourceService.LoadObjectAsync<TileController>(_gameWorldService.MapObject);
            tileController.Initialize(tileModel);
            tileController.transform.position = tileModel.Position.ToVector3();
            _mapTilesControllers.Add(tileModel, tileController);
            return tileController;
        }
    }
}