using Cysharp.Threading.Tasks;

namespace Game.Common.Controller
{
    public interface IInteractableComponent
    {
        UniTask Interact();
        UniTask StopInteract();
    }
}