using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Tile
{
    [Handler("Tile")]
    public class TileInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(TileInteractionHandler)}");
        }
    }
}