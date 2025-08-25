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
            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionName, pieMenuSettings.Title, pieMenuSettings.Description, sprite, new());
            return UniTask.FromResult(itemModel);
        }
    }
}