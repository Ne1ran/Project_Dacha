using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("FertilizePlant")]
    public class FertilizePlantInteractionHandler : IInteractionHandler
    {
        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(FertilizePlantInteractionHandler)}");
            return UniTask.CompletedTask;
        }
    }
}