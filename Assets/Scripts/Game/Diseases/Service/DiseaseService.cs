using Core.Attributes;
using Core.Descriptors.Service;

namespace Game.Diseases.Service
{
    [Service]
    public class DiseaseService
    {
        private readonly IDescriptorService _descriptorService;

        public DiseaseService(IDescriptorService descriptorService)
        {
            _descriptorService = descriptorService;
        }
    }
}