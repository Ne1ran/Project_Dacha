using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Plants.Service;
using Game.Soil.Service;
using VContainer;

namespace Game.Seeds.Handlers
{
    [Handler("SowSeed")]
    public class SeedSowHandler : ISowSeedHandler
    {
        [Inject]
        private readonly PlantsService _plantsService = null!;
        [Inject]
        private readonly SoilService _soilService = null!;

        public UniTask SowSeedAsync(string seedId, Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId);
            if (!_soilService.TrySowSeed(tileId)) {
                return UniTask.CompletedTask;
            }
            
            _plantsService.CreatePlant(seedId, tileId);
            return UniTask.CompletedTask;
        }
    }
}