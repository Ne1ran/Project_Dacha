using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Service;
using VContainer;

namespace Game.Tools.Handlers
{
    [Handler("Hoe")]
    public class UseHoeHandler : IUseToolHandler
    {
        [Inject]
        private readonly SoilService _soilService = null!;
        
        public UniTask UseAsync(Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId);
            _soilService.TiltSoil(tileId);
            return UniTask.CompletedTask;
        }
    }
}