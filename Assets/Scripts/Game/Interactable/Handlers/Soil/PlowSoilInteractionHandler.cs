using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Soil
{
    [Handler("PlowSoil")]
    public class PlowSoilInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(PlowSoilInteractionHandler)}");
        }
    }
}