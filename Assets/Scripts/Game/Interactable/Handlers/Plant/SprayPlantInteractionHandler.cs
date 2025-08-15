using Core.Parameters;
using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("SprayPlant")]
    public class SprayPlantInteractionHandler : IInteractionHandler
    {
        public void Interact(Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(SprayPlantInteractionHandler)}");
        }
    }
}