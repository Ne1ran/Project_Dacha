using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using UnityEngine;

namespace Game.Interactable.Handlers.Environment
{
    [Handler("CloseDoor")]
    public class CloseDoorInteractionHandler : IInteractionHandler
    {
        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(CloseDoorInteractionHandler)}");
            return UniTask.CompletedTask;
        }
    }
}