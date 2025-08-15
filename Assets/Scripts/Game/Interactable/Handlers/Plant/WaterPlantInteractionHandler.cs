using Core.Parameters;
using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("WaterPlant")]
    public class WaterPlantInteractionHandler : IInteractionHandler
    {
        public void Interact(Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(WaterPlantInteractionHandler)}");
        }
    }
}