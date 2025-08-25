using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Common.Handlers;
using Game.Interactable.Descriptor;
using Game.PieMenu.Model;

namespace Game.PieMenu.PrepareHandlers
{
    [Handler("Simple")]
    public class SimplePieMenuPrepareHandler : IPieMenuPrepareHandler
    {
        public UniTask<PieMenuItemModel> Prepare(InteractionPieMenuSettings pieMenuSettings, CancellationToken token)
        {
            List<PieMenuItemSelectionModel> models = new() {
                    new(string.Empty, pieMenuSettings.BaseIcon, string.Empty)
            };

            PieMenuItemModel itemModel = new(pieMenuSettings.InteractionHandlerName, pieMenuSettings.Title, pieMenuSettings.Description, models);
            return UniTask.FromResult(itemModel);
        }
    }
}