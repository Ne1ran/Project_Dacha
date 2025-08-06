using Core.Console.Controller;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Core.Console.Service
{
    [UsedImplicitly]
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