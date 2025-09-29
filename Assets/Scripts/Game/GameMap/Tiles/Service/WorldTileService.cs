using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.GameWorld.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Tiles.Component;
using Game.GameMap.Tiles.Model;
using Game.Plants.Event;
using Game.Soil.Event;
using MessagePipe;
using Math = System.Math;

namespace Game.GameMap.Tiles.Service
{
    [Service]
    public class WorldTileService : IDisposable
    {
        private readonly IResourceService _resourceService;
        private readonly GameWorldService _gameWorldService;
        private readonly IDescriptorService _descriptorService;

        private readonly List<SingleTileModel> _mapTilesModels = new();
        private readonly Dictionary<string, TileController> _mapTilesControllers = new();

        private IDisposable? _disposable;

        public WorldTileService(IResourceService resourceService,
                                GameWorldService gameWorldService,
                                IDescriptorService descriptorService,
                                ISubscriber<string, SoilControllerCreatedEvent> soilCreatedSubscriber,
                                ISubscriber<string, PlantControllerCreatedEvent> plantCreatedSubscriber)
        {
            _resourceService = resourceService;
            _gameWorldService = gameWorldService;
            _descriptorService = descriptorService;

            DisposableBagBuilder? disposableBag = DisposableBag.CreateBuilder();
            disposableBag.Add(soilCreatedSubscriber.Subscribe(SoilControllerCreatedEvent.SoilCreated, OnSoilCreated));
            disposableBag.Add(plantCreatedSubscriber.Subscribe(PlantControllerCreatedEvent.PlantCreated, OnPlantCreated));
            _disposable = disposableBag.Build();

            // todo neiran subscribe on tile/soil/everything changes to change soil visualization
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
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

        public Dictionary<string, int> GetNearbyTiles(string centerTileId, int range)
        {
            Dictionary<string, int> result = new();

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

                    result.Add(_mapTilesModels[neighborIndex].Id, minRange);
                }
            }

            return result;
        }

        private async UniTask<TileController> CreateTileAsync(SingleTileModel tileModel)
        {
            TileController tileController = await _resourceService.InstantiateAsync<TileController>(_gameWorldService.MapObject);
            tileController.Initialize(tileModel);
            tileController.transform.position = tileModel.Position.ToVector3();
            _mapTilesControllers.Add(tileModel.Id, tileController);
            return tileController;
        }

        private void OnSoilCreated(SoilControllerCreatedEvent evt)
        {
            if (!_mapTilesControllers.TryGetValue(evt.Id, out TileController tileController)) {
                throw new ArgumentException($"Tile controller not found for tile model. Id={evt.Id}");
            }
            
            tileController.AddSoil(evt.SoilController);
        }

        private void OnPlantCreated(PlantControllerCreatedEvent evt)
        {
            if (!_mapTilesControllers.TryGetValue(evt.TileId, out TileController tileController)) {
                throw new ArgumentException($"Tile controller not found for tile model. Id={evt.TileId}");
            }
            
            tileController.AddPlant(evt.PlantController);
        }
    }
}