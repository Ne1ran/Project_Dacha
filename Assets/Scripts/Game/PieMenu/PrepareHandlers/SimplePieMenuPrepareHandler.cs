using System.Collections.Generic;
using System.Threading;
using Core.Resources.Service;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Interactable.Descriptor;
using Game.PieMenu.Model;
using UnityEngine;
using VContainer;

namespace Game.PieMenu.PrepareHandlers
{
    [Handler("Simple")]
    public class SimplePieMenuPrepareHandler : IPieMenuPrepareHandler
    {
        [Inject]
        private readonly IResourceService _resourceService = null!;

        public async UniTask<PieMenuItemModel> PrepareAsync(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            Sprite? itemIcon = pieMenuSettings.BaseIcon != null
                                       ? await _resourceService.LoadAssetAsync<Sprite>(pieMenuSettings.BaseIcon.AssetGUID, token)
                                       : null;
            List<PieMenuItemSelectionModel> models = new() {
                    new(string.Empty, itemIcon, string.Empty)
            };

            return new(pieMenuSettings.InteractionHandlerName, pieMenuSettings.Title, pieMenuSettings.Description, models);
        }
    }
}