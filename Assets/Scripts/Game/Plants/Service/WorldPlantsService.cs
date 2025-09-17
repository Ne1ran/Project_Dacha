using System;
using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Tiles.Model;
using Game.Plants.Component;
using Game.Plants.Event;
using Game.Plants.Model;
using Game.Utils;
using MessagePipe;
using UnityEngine;

namespace Game.Plants.Service
{
    [Service]
    public class WorldPlantsService : IDisposable
    {
        private readonly IResourceService _resourceService;
        private readonly IDescriptorService _descriptorService;
        private readonly PlantsService _plantsService;
        private readonly IPublisher<string, PlantControllerCreatedEvent> _plantControllerCreatedPublisher;

        private readonly Dictionary<string, PlantController> _plantControllers = new();

        private IDisposable? _disposable;

        public WorldPlantsService(IResourceService resourceService,
                                  IDescriptorService descriptorService,
                                  PlantsService plantsService,
                                  ISubscriber<string, PlantUpdatedEvent> plantUpdatedSubscriber,
                                  IPublisher<string, PlantControllerCreatedEvent> plantControllerCreatedPublisher)
        {
            _resourceService = resourceService;
            _descriptorService = descriptorService;
            _plantsService = plantsService;
            _plantControllerCreatedPublisher = plantControllerCreatedPublisher;

            DisposableBagBuilder bag = DisposableBag.CreateBuilder();
            bag.Add(plantUpdatedSubscriber.Subscribe(PlantUpdatedEvent.Created, OnPlantCreated));
            bag.Add(plantUpdatedSubscriber.Subscribe(PlantUpdatedEvent.Updated, OnPlantUpdated));
            bag.Add(plantUpdatedSubscriber.Subscribe(PlantUpdatedEvent.Removed, OnPlantRemoved));
            _disposable = bag.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public async UniTask<List<PlantController>> CreatePlants(List<SingleTileModel> tiles)
        {
            List<UniTask<PlantController>> list = new();

            foreach (SingleTileModel tile in tiles) {
                PlantModel? plantModel = _plantsService.GetPlant(tile.Id);
                if (plantModel == null) {
                    continue;
                }

                list.Add(CreatePlantController(tile.Id, plantModel));
            }

            if (list.Count == 0) {
                return new();
            }

            PlantController[] controllers = await UniTask.WhenAll(list);
            return controllers.ToList();
        }

        private async UniTask<PlantController> CreatePlantController(string tileId, PlantModel plantModel)
        {
            PlantController plantController = await _resourceService.LoadObjectAsync<PlantController>();
            await plantController.InitializeAsync(tileId, plantModel);
            _plantControllers.Add(tileId, plantController);
            _plantControllerCreatedPublisher.Publish(PlantControllerCreatedEvent.PlantCreated, new(tileId, plantController));
            return plantController;
        }

        private void OnPlantCreated(PlantUpdatedEvent evt)
        {
            CreatePlantController(evt.TileId, evt.PlantModel).Forget();
        }

        private void OnPlantUpdated(PlantUpdatedEvent evt)
        {
            if (!_plantControllers.TryGetValue(evt.TileId, out PlantController plantController)) {
                Debug.LogWarning($"Can't remove plant if it doesn't exist! TileId={evt.TileId}, PlantId={evt.PlantModel.PlantId}");
                return;
            }

            plantController.UpdatePlantsIfNeeded(evt.PlantModel);
        }

        private void OnPlantRemoved(PlantUpdatedEvent evt)
        {
            if (!_plantControllers.TryGetValue(evt.TileId, out PlantController plantController)) {
                Debug.LogWarning($"Can't remove plant if it doesn't exist! TileId={evt.TileId}, PlantId={evt.PlantModel.PlantId}");
                return;
            }

            plantController.DestroyObject();
            _plantControllers.Remove(evt.TileId);
        }
    }
}