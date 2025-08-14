using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Environment
{
    [Handler("CloseDoor")]
    public class CloseDoorInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(CloseDoorInteractionHandler)}");
        }
    }
}