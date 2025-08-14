using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers
{
    [Handler("OpenDoor")]
    public class OpenDoorInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(OpenDoorInteractionHandler)}");
        }
    }
}