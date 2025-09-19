using Core.Attributes;
using Core.Descriptors.Service;
using Core.Resources.Service;

namespace Game.Fertilizers.Service
{
    [Service]
    public class FertilizerService
    {
        private readonly IDescriptorService _descriptorService;
        private readonly IResourceService _resourceService;

        public FertilizerService(IDescriptorService descriptorService, IResourceService resourceService)
        {
            _descriptorService = descriptorService;
            _resourceService = resourceService;
        }
    }
}