using System.Collections.Generic;
using Core.Parameters;
using Game.Common.Handlers;
using Game.GameMap.Tiles.Service;
using Game.Inventory.Model;
using Game.Inventory.Service;
using UnityEngine;
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

            List<InventoryItem> hotkeyItems = _inventoryService.GetHotkeyItems();
            // todo neiran redo afterwards to conditions?
            foreach (InventoryItem inventoryItem in hotkeyItems) {
                if (inventoryItem.ItemType != ItemType.FERTILIZER) {
                    continue;
                }
                
                UseFertilizer(tileId, fertilizerId, portionMass);
                return;
            }

            // todo neiran show notifications
            Debug.LogWarning("No fertilizer found in inventory!");
        }

        private void UseFertilizer(string tileId, string fertilizerId, float portionMass)
        {
            _tileService.AddFertilizer(tileId, fertilizerId, portionMass);
        }
    }
}