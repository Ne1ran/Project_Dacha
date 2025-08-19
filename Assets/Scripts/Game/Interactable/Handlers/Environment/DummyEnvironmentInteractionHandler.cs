using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using UnityEngine;

namespace Game.Interactable.Handlers.Environment
{
    [Handler("DummyEnvironment")]
    public class DummyEnvironmentInteractionHandler : IInteractionHandler
    {
        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(DummyEnvironmentInteractionHandler)}");
            return UniTask.CompletedTask;
        }
    }
}