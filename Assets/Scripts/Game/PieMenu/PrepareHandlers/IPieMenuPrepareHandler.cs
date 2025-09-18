using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Interactable.Descriptor;
using Game.PieMenu.Model;

namespace Game.PieMenu.PrepareHandlers
{
    public interface IPieMenuPrepareHandler
    {
        UniTask<PieMenuItemModel> PrepareAsync(InteractionPieMenuSettings pieMenuSettings, CancellationToken token);
    }
}