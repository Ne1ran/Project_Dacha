using Core.Descriptors.Service;
using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Descriptor;
using Game.GameMap.Soil.Model;
using Game.GameMap.Soil.Service;
using UnityEngine;
using VContainer;

namespace Game.Tools.Handlers
{
    [Handler("Shovel")]
    public class UseShovelHandler : IUseToolHandler
    {
        [Inject]
        private readonly SoilService _soilService = null!;
        [Inject]
        private readonly IDescriptorService _descriptorService = null!;
        
        public UniTask UseAsync(Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId);
            SoilModel soilModel = _soilService.GerOrCreate(tileId);
            if (soilModel.State == SoilState.Planted) {
                Debug.LogWarning("Can't shovel soil when something is planted on it!"); // todo neiran notification??
                return UniTask.CompletedTask;
            }

            if (soilModel.DugRecently) {
                Debug.LogWarning("Can't shovel twice on same day! (Don't need to)"); // todo neiran notification??
                return UniTask.CompletedTask;
            }

            SoilDescriptor soilDescriptor = _descriptorService.Require<SoilDescriptor>();
            SoilDescriptorModel soilDescriptorModel = soilDescriptor.RequireByType(soilModel.Type);

            float minWaterAmount = soilDescriptorModel.StartWaterAmount / 2f;
            float minBreathability = soilDescriptorModel.Breathability;
            soilModel.WaterAmount = Mathf.Max(minWaterAmount, soilModel.WaterAmount * 0.75f);
            soilModel.Breathability = Mathf.Min(minBreathability, soilModel.Breathability * 1.25f);
            soilModel.State = SoilState.None;
            soilModel.DugRecently = true;
            return UniTask.CompletedTask;
        }
    }
}