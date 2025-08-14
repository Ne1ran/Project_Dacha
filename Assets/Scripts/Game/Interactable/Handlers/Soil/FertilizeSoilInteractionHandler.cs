using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("FertilizeSoil")]
    public class FertilizeSoilInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(FertilizeSoilInteractionHandler)}");
        }
    }
}