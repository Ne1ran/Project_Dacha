using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("FertilizePlant")]
    public class FertilizePlantInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(FertilizePlantInteractionHandler)}");
        }
    }
}