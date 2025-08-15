using Core.Parameters;
using Game.Common.Handlers;
using UnityEngine;

namespace Game.Interactable.Handlers.Environment
{
    [Handler("DummyEnvironment")]
    public class DummyEnvironmentInteractionHandler : IInteractionHandler
    {
        public void Interact(Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(DummyEnvironmentInteractionHandler)}");
        }
    }
}