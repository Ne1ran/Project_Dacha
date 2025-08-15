using Core.Parameters;
using Game.Common.Handlers;
using Game.GameMap.Tiles.Service;
using Game.Inventory.Service;
using VContainer;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("FertilizeSoil")]
    public class FertilizeSoilInteractionHandler : IInteractionHandler
    {
        [Inject]
        private readonly TileService _tileService = null!;
        [Inject]
        private readonly InventoryService _inventoryService = null!;

        public void Interact(Parameters parameters)
        {
            string tileId = parameters.Require<string>(ParameterNames.TileId); // found tile to do smth on it
            string fertilizerId = parameters.Require<string>(ParameterNames.ItemId);
            float portionMass = parameters.Require<float>(ParameterNames.PortionMass);
            UseFertilizer(tileId, fertilizerId, portionMass);
        }

        private void UseFertilizer(string tileId, string fertilizerId, float portionMass)
        {
            _tileService.AddFertilizer(tileId, fertilizerId, portionMass);
        }
    }
}