using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers
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