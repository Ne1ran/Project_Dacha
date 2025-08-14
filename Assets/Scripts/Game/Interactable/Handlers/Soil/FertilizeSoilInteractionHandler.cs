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

            List<InventoryItem> hotkeyItems = _inventoryService.GetHotkeyItems();
            // todo neiran redo afterwards
            foreach (InventoryItem inventoryItem in hotkeyItems) {
                if (inventoryItem.ItemType == ItemType.FERTILIZER) {
                    UseFertilizer();
                    return;
                }
            }

            // todo neiran show notifications
            Debug.LogWarning("No fertilizer found in inventory!");
        }

        private void UseFertilizer()
        {
            Debug.LogWarning("Using fertilizer from inventory!");
        }
    }
}