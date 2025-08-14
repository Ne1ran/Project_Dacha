using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("WaterSoil")]
    public class WaterSoilInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(WaterSoilInteractionHandler)}");
        }
    }
}