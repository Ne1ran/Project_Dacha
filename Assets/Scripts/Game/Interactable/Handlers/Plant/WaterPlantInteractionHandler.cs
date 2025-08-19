using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.PieMenu.Model;
using UnityEngine;

namespace Game.Interactable.Handlers.Plant
{
    [Handler("WaterPlant")]
    public class WaterPlantInteractionHandler : IInteractionHandler
    {
        public UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters)
        {
            Debug.LogWarning($"Interaction with={nameof(WaterPlantInteractionHandler)}");
            return UniTask.CompletedTask;
        }
    }
}