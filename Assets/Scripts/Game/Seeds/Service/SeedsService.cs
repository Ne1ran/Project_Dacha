using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Parameters;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Seeds.Component;
using Game.Seeds.Descriptors;
using UnityEngine;

namespace Game.Seeds.Service
{
    [Service]
    public class SeedsService
    {
        private readonly SowSeedHandlerFactory _sowSeedHandlerFactory;
        private readonly IResourceService _resourceService;
        private readonly IDescriptorService _descriptorService;

        public SeedsService(IDescriptorService descriptorService, IResourceService resourceService, SowSeedHandlerFactory sowSeedHandlerFactory)
        {
            _descriptorService = descriptorService;
            _resourceService = resourceService;
            _sowSeedHandlerFactory = sowSeedHandlerFactory;
        }

        public async UniTask<SeedBagController> CreateSeedBag(string seedId, Vector3 position)
        {
            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();
            List<SeedsDescriptorModel> seeds = seedsDescriptor.Items;
            SeedsDescriptorModel seedsDescriptorModel = seeds.Find(seed => seed.SeedId == seedId);
            if (seedsDescriptorModel == null) {
                throw new ArgumentException($"Seed not found with id={seedId}");
            }

            SeedBagController seedBagController = await _resourceService.InstantiateAsync<SeedBagController>(seedsDescriptorModel.SeedPrefab);
            seedBagController.transform.position = position;
            seedBagController.name = seedsDescriptorModel.SeedId;
            return seedBagController;
        }

        public async UniTask<SeedBagController> CreateSeedBag(string seedId, Transform parent)
        {
            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();
            List<SeedsDescriptorModel> seeds = seedsDescriptor.Items;
            SeedsDescriptorModel seedsDescriptorModel = seeds.Find(seed => seed.SeedId == seedId);
            if (seedsDescriptorModel == null) {
                throw new ArgumentException($"Seed not found with id={seedId}");
            }

            SeedBagController seedBagController = await _resourceService.InstantiateAsync<SeedBagController>(seedsDescriptorModel.SeedPrefab);
            seedBagController.transform.SetParent(parent);
            seedBagController.name = seedsDescriptorModel.SeedId;
            return seedBagController;
        }

        public async UniTask SowSeedAsync(string seedId, Parameters parameters)
        {
            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();
            List<SeedsDescriptorModel> seeds = seedsDescriptor.Items;
            SeedsDescriptorModel? seedsDescriptorModel = seeds.Find(seed => seed.SeedId == seedId);
            if (seedsDescriptorModel == null) {
                throw new ArgumentException($"Seed not found with id={seedId}");
            }
            
            await _sowSeedHandlerFactory.Create(seedsDescriptorModel.UseHandler).SowSeedAsync(seedsDescriptorModel.SeedId, parameters);
        }
    }
}