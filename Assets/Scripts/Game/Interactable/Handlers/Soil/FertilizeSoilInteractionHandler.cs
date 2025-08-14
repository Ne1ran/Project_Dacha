using Core.Parameters;
using Game.Common.Handlers;
using Game.GameMap.Tiles.Service;
using VContainer;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("FertilizeSoil")]
    public class FertilizeSoilInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly TileService _tileService = null!;

        public void Interact(Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId);
        }
    }
}