using Core.Parameters;
using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Environment
{
    [Handler("OpenDoor")]
    public class OpenDoorInteractionHandler : IInteractionHandler
    {
        public void Interact(Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(OpenDoorInteractionHandler)}");
        }
    }
}