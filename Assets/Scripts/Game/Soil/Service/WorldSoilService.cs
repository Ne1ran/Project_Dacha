using System;
using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Difficulty.Model;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Tiles.Event;
using Game.GameMap.Tiles.Model;
using Game.Soil.Component;
using Game.Soil.Descriptor;
using Game.Soil.Event;
using Game.Soil.Model;
using MessagePipe;

namespace Game.Soil.Service
{
    [Service]
    public class WorldSoilService : IDisposable
    {
        private readonly IResourceService _resourceService;
        private readonly IDescriptorService _descriptorService;
        private readonly IPublisher<string, SoilControllerCreatedEvent> _soilControllerCreatedPublisher;

        private readonly Dictionary<string, SoilController> _soilControllers = new();

        private IDisposable? _disposable;

        public WorldSoilService(IResourceService resourceService,
                                IDescriptorService descriptorService,
                                IPublisher<string, SoilControllerCreatedEvent> soilControllerCreatedPublisher,
                                ISubscriber<string, SoilUpdatedEvent> soilUpdatedSubscriber)
        {
            _resourceService = resourceService;
            _descriptorService = descriptorService;
            _soilControllerCreatedPublisher = soilControllerCreatedPublisher;

            DisposableBagBuilder disposableBag = DisposableBag.CreateBuilder();
            disposableBag.Add(soilUpdatedSubscriber.Subscribe(SoilUpdatedEvent.Updated, OnSoilUpdated));
            _disposable = disposableBag.Build();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public async UniTask<List<SoilController>> CreateSoilControllers(List<SingleTileModel> tiles)
        {
            MapDescriptor mapDescriptor = _descriptorService.Require<MapDescriptor>();
            SoilDescriptor soilDescriptor = _descriptorService.Require<SoilDescriptor>();
            SoilType defaultSoilType = mapDescriptor.Require(DachaPlaceType.Middle).SoilType;
            SoilDescriptorModel soilDescriptorModel = soilDescriptor.Require(defaultSoilType);
            SoilVisualDescriptor soilVisualDescriptor = soilDescriptorModel.SoilVisualDescriptor;

            List<UniTask<SoilController>> soilControllers = new();
            foreach (SingleTileModel tileModel in tiles) {
                soilControllers.Add(CreateSoil(tileModel.Id, soilVisualDescriptor));
            }

            SoilController[] controllers = await UniTask.WhenAll(soilControllers);
            return controllers.ToList();
        }

        private async UniTask<SoilController> CreateSoil(string soilId, SoilVisualDescriptor soilVisualDescriptor)
        {
            SoilController soil = await _resourceService.InstantiateAsync<SoilController>();
            await soil.InitializeAsync(soilId, soilVisualDescriptor);
            _soilControllerCreatedPublisher.Publish(SoilControllerCreatedEvent.SoilCreated, new(soilId, soil));
            _soilControllers.Add(soilId, soil);
            return soil;
        }

        private void OnSoilUpdated(SoilUpdatedEvent evt)
        {
            if (!_soilControllers.TryGetValue(evt.Id, out SoilController soilController)) {
                throw new KeyNotFoundException($"Soil controller not found. Id={evt.Id}");
            }
            
            soilController.UpdateSkinAsync().Forget();
        }
    }
}