using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.GameMap.Map.Descriptor;
using Game.GameMap.Soil.Component;
using Game.GameMap.Soil.Descriptor;
using Game.GameMap.Soil.Event;
using Game.GameMap.Soil.Model;
using Game.GameMap.Tiles.Model;
using MessagePipe;
using UnityEngine;

namespace Game.GameMap.Soil.Service
{
    [Service]
    public class WorldSoilService
    {
        private readonly IResourceService _resourceService;
        private readonly IDescriptorService _descriptorService;
        private readonly IPublisher<string, SoilControllerCreatedEvent> _soilControllerCreatedPublisher;
        private readonly SoilService _soilService;

        private readonly Dictionary<string, SoilController> _soilControllers = new();

        public WorldSoilService(IResourceService resourceService,
                                IDescriptorService descriptorService,
                                SoilService soilService,
                                IPublisher<string, SoilControllerCreatedEvent> soilControllerCreatedPublisher)
        {
            _resourceService = resourceService;
            _descriptorService = descriptorService;
            _soilService = soilService;
            _soilControllerCreatedPublisher = soilControllerCreatedPublisher;
        }

        public async UniTask<List<SoilController>> CreateSoilControllers(List<SingleTileModel> tiles)
        {
            MapDescriptor mapDescriptor = _descriptorService.Require<MapDescriptor>();
            SoilDescriptor soilDescriptor = _descriptorService.Require<SoilDescriptor>();
            SoilType defaultSoilType = mapDescriptor.SoilType;
            SoilDescriptorModel soilDescriptorModel = soilDescriptor.RequireByType(defaultSoilType);
            SoilVisualDescriptor soilVisualDescriptor = soilDescriptorModel.SoilVisualDescriptor;

            List<UniTask<SoilController>> soilControllers = new();
            foreach (SingleTileModel tileModel in tiles) {
                soilControllers.Add(CreateSoil(tileModel, soilVisualDescriptor));
            }

            SoilController[] controllers = await UniTask.WhenAll(soilControllers);
            return controllers.ToList();
        }

        private async UniTask<SoilController> CreateSoil(SingleTileModel tileModel, SoilVisualDescriptor soilVisualDescriptor)
        {
            SoilController soil = await _resourceService.LoadObjectAsync<SoilController>();

            string soilId = tileModel.Id;
            SoilModel? soilModel = _soilService.GetSoil(soilId);
            string prefabPath = soilModel == null
                                        ? soilVisualDescriptor.BaseViewPrefab
                                        : soilVisualDescriptor.GetPrefabPath(soilModel.State, soilModel.DugRecently, soilModel.WellWatered);
            Transform skin = await _resourceService.LoadObjectAsync<Transform>(prefabPath);
            soil.Initialize(skin);

            _soilControllerCreatedPublisher.Publish(SoilControllerCreatedEvent.SoilCreated, new(tileModel, soil));
            _soilControllers.Add(soilId, soil);
            return soil;
        }
    }
}