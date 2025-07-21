using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Common.Controller
{
    public class PickableComponent : MonoBehaviour, IInteractableComponent
    {
        public UniTask Interact()
        {
            Debug.Log($"Pick up ={gameObject.name}");
            Destroy(gameObject);
            return UniTask.CompletedTask;
        }
    }
}