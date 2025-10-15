using Core.Attributes;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Seeds.Descriptors;

namespace Game.Seeds.Service
{
    [Service]
    public class SeedsService
    {
        private readonly SowSeedHandlerFactory _sowSeedHandlerFactory;
        private readonly SeedsDescriptor _seedsDescriptor;

        public SeedsService(SowSeedHandlerFactory sowSeedHandlerFactory, SeedsDescriptor seedsDescriptor)
        {
            _sowSeedHandlerFactory = sowSeedHandlerFactory;
            _seedsDescriptor = seedsDescriptor;
        }

        public async UniTask SowSeedAsync(string seedId, Parameters parameters)
        {
            SeedsDescriptorModel seedsDescriptorModel = _seedsDescriptor.Require(seedId);
            await _sowSeedHandlerFactory.Create(seedsDescriptorModel.UseHandler).SowSeedAsync(seedId, parameters);
        }
    }
}