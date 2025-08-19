using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.GameMap.Soil.Service;
using Game.PieMenu.Model;
using Game.Tools.Service;
using VContainer;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("UseToolOnSoil")]
    public class UseToolOnSoilInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly SoilService _soilService = null!;
        [Inject]
        private readonly ToolsService _toolsService = null!;

        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId);
            string toolId = parameters.Require<string>(ParameterNames.ItemId);
            // _toolsService.UseToolAsync(tileId, toolId).Forget();
            return UniTask.CompletedTask;
        }
    }
}