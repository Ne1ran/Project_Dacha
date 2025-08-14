using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Inventory.Service;
using Game.Items.Service;
using Game.Utils;
using UnityEngine;
using VContainer;

namespace Game.Items.Controller
{
    public class ItemController : MonoBehaviour, IInteractableComponent
    {
        [Inject]
        private PickUpItemService _pickUpItemService = null!;
        
        private Rigidbody _rigidbody = null!;

        private void Awake()
        {
            _rigidbody = this.RequireComponent<Rigidbody>();
        }

        public bool IsKinematic
        {
            get => _rigidbody.isKinematic;
            set => _rigidbody.isKinematic = value;
        }

        public UniTask Interact()
        {
            _pickUpItemService.PickUpItem(this);
            return UniTask.CompletedTask;
        }

        public UniTask StopInteract()
        {
            return UniTask.CompletedTask;
        }

        public string GetName => gameObject.name;
    }
}