using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("InspectPlant")]
    public class InspectPlantInteractionHandler : IInteractionHandler
    {
        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(InspectPlantInteractionHandler)}");
            return UniTask.CompletedTask;
        }
    }
}