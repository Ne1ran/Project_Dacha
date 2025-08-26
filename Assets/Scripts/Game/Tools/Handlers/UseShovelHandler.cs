using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Service;
using VContainer;

namespace Game.Tools.Handlers
{
    [Handler("Shovel")]
    public class UseShovelHandler : IUseToolHandler
    {
        [Inject]
        private readonly SoilService _soilService = null!;
        
        public UniTask UseAsync(Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId);
            _soilService.TryShovelSoil(tileId);
            return UniTask.CompletedTask;
        }
    }
}