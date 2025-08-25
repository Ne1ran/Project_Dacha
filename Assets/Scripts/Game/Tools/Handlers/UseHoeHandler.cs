using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using UnityEngine;
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
            SoilModel soilModel = _soilService.GerOrCreate(tileId);
            if (soilModel.State == SoilState.Planted) {
                Debug.LogWarning("Can't tilt soil when something is planted on it!"); // todo neiran notification??
                return UniTask.CompletedTask;
            }

            soilModel.State = SoilState.Tilted;
            return UniTask.CompletedTask;
        }
    }
}