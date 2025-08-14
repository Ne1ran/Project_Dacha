using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Environment
{
    [Handler("DummyEnvironment")]
    public class DummyEnvironmentInteractionHandler : IInteractionHandler
    {
        public void Interact()
        {
            Debug.LogWarning($"Interaction with={nameof(DummyEnvironmentInteractionHandler)}");
        }
    }
}