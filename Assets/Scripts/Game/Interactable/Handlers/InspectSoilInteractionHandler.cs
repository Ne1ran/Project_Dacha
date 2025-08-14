using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers
{
    [Handler("InspectSoil")]
    public class InspectSoilInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(InspectSoilInteractionHandler)}");
        }
    }
}