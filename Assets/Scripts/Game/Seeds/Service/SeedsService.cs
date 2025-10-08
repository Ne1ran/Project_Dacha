using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Descriptors.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Seeds.Descriptors;
using Game.Utils;

namespace Game.Seeds.Service
{
    [Service]
    public class SeedsService
    {
        private readonly SowSeedHandlerFactory _sowSeedHandlerFactory;
        private readonly IDescriptorService _descriptorService;

        public SeedsService(IDescriptorService descriptorService, SowSeedHandlerFactory sowSeedHandlerFactory)
        {
            _descriptorService = descriptorService;
            _sowSeedHandlerFactory = sowSeedHandlerFactory;
        }

        public async UniTask SowSeedAsync(string seedId, Parameters parameters)
        {
            SeedsDescriptor seedsDescriptor = _descriptorService.Require<SeedsDescriptor>();
            SeedsDescriptorModel seedsDescriptorModel = seedsDescriptor.Require(seedId);
            await _sowSeedHandlerFactory.Create(seedsDescriptorModel.UseHandler).SowSeedAsync(seedsDescriptorModel.Id, parameters);
        }
    }
}