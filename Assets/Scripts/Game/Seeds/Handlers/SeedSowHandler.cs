using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using Game.Plants.Service;
using UnityEngine;
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
            SoilModel soilModel = _soilService.GerOrCreate(tileId);
            if (soilModel.State == SoilState.Planted) {
                Debug.LogWarning("Can't sow seed on a planted soil!");
                return UniTask.CompletedTask;
            }
            
            soilModel.State = SoilState.Planted;
            _plantsService.CreatePlant(seedId, tileId);
            return UniTask.CompletedTask;
        }
    }
}