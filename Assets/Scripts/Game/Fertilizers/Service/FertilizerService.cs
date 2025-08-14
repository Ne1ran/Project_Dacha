using System;
using Core.Descriptors.Service;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Fertilizers.Controller;
using Game.Fertilizers.Descriptor;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Fertilizers.Service
{
    [UsedImplicitly]
    public class FertilizerService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly IResourceService _resourceService;

        public FertilizerService(IDescriptorService descriptorService, IResourceService resourceService)
        {
            _descriptorService = descriptorService;
            _resourceService = resourceService;
        }

        public async UniTask<FertilizerController> CreateFertilizer(string fertilizerId, Vector3 position)
        {
            FertilizersDescriptor fertilizersDescriptor = _descriptorService.Require<FertilizersDescriptor>();
            FertilizerDescriptorModel? fertModel = fertilizersDescriptor.Fertilizers.Find(fert => fert.Id == fertilizerId);
            if (fertModel == null) {
                throw new ArgumentException($"Fertilizer not found with id={fertilizerId}");
            }

            FertilizerController toolController = await _resourceService.LoadObjectAsync<FertilizerController>(fertModel.PrefabPath);
            toolController.transform.position = position;
            toolController.name = fertilizerId; // todo redo
            return toolController;
        }
    }
}