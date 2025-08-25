using Core.Parameters;
using Cysharp.Threading.Tasks;
using Game.PieMenu.Model;

namespace Game.Interactable.Handlers
{
    public interface IInteractionHandler
    {
        UniTask InteractAsync(PieMenuItemModel itemModel, Parameters parameters);
    }
}