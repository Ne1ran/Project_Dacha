using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Items.Service;
using UnityEngine;
using VContainer;

namespace Game.Items.Controller
{
    [NeedBinding]
    public class ItemController : MonoBehaviour, IInteractableComponent
    {
        [Inject]
        private PickUpItemService _pickUpItemService = null!;

        [ComponentBinding]
        private Rigidbody _rigidbody = null!;

        private string _itemId = null!;
        
        public void Initialize(string itemId)
        {
            _itemId = itemId;
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

        public string ItemId => _itemId;
    }
}