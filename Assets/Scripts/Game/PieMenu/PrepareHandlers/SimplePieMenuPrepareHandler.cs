using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Interactable.Descriptor;
using Game.PieMenu.Model;
using UnityEngine;

namespace Game.PieMenu.PrepareHandlers
{
    [Handler("Simple")]
    public class SimplePieMenuPrepareHandler : IPieMenuPrepareHandler
    {
        public UniTask<PieMenuItemModel> Prepare(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            Sprite? sprite = Resources.Load<Sprite>(pieMenuSettings.IconPath); // todo neiran remove when go to addressables!!!

            List<PieMenuItemSelectionModel> models = new() {
                    new(string.Empty, sprite, string.Empty)
            };

            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionName, pieMenuSettings.Title, pieMenuSettings.Description, models);
            return UniTask.FromResult(itemModel);
        }
    }
}