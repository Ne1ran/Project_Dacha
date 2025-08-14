using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers
{
    [Handler("Plant")]
    public class PlantInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(PlantInteractionHandler)}");
        }
    }
}