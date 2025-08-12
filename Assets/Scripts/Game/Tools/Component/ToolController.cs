using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Items.Controller;
using Game.Tools.Service;
using UnityEngine;
using VContainer;

namespace Game.Tools.Component
{
    public class ToolController : ItemController, IInteractableComponent
    {
        [Inject]
        private ToolsService _toolsService = null!;

        public UniTask Interact()
        {
            _toolsService.PickUpTool(this);
            return UniTask.CompletedTask;
        }

        public UniTask StopInteract()
        {
            Debug.Log($"Stopped interacting with tool={GetName}!");
            return UniTask.CompletedTask;
        }

        public string GetName => gameObject.name;
    }
}