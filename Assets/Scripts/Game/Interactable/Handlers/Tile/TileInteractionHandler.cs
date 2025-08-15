using Core.Parameters;
using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Tile
{
    [Handler("Tile")]
    public class TileInteractionHandler : IInteractionHandler
    {
        public void Interact(Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(TileInteractionHandler)}");
        }
    }
}