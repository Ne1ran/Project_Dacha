using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("WaterPlant")]
    public class WaterPlantInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(WaterPlantInteractionHandler)}");
        }
    }
}