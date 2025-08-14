using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("InspectPlant")]
    public class InspectPlantInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(InspectPlantInteractionHandler)}");
        }
    }
}