using Core.Attributes;
using Core.Console.Controller;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;

namespace Core.Console.Service
{
    [Service]
    public class ConsoleService
    {
        private readonly IResourceService _resourceService;

        public ConsoleService(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public async UniTask InitializeAsync()
        {
            await _resourceService.LoadObjectAsync<ConsoleController>();
        }
    }
}