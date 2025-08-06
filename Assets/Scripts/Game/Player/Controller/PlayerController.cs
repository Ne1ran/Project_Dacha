using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Inventory.Event;
using Game.Movement;
using Game.PlayMode.Service;
using Game.Spawn;
using Game.Utils;
using MessagePipe;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace Game.Player.Controller
{
    [PrefabPath("Prefabs/Player/Player")]
    public class PlayerController : MonoBehaviour
    {
        [Inject]
        private PlayModeService _playModeService;
        [Inject]
        private IPublisher<string, InventoryStatusEvent> _inventoryStatusPublisher;

        private PickUpComponent _pickUpComponent = null!;
        private MovementController _movementController = null!;

        private bool _cursorEnabled = true;
        private bool _inventoryEnabled = true;

        private Transform _headCamera = null!;

        private LayerMask _layerMask;

        private void Awake()
        {
            _headCamera = this.RequireComponentInChildren<Camera>().transform;
            _pickUpComponent = this.AddComponent<PickUpComponent>();
            _movementController = this.RequireComponent<MovementController>();

            _pickUpComponent.OnLook += OnPickUpStarted;
            _pickUpComponent.OnUnlook += OnPickUpFinished;
            _pickUpComponent.OnInteract += OnInteract;
        }

        private void OnDestroy()
        {
            _pickUpComponent.OnLook -= OnPickUpStarted;
            _pickUpComponent.OnUnlook -= OnPickUpFinished;
            _pickUpComponent.OnInteract -= OnInteract;
        }

        private void OnPickUpStarted()
        {
            _playModeService.PlayModeScreen.ShowCrosshair(true);
        }

        private void OnPickUpFinished()
        {
            _playModeService.PlayModeScreen.FadeCrosshair(true);
        }

        private void OnInteract(IInteractableComponent target)
        {
            target.Interact().Forget();
        }

        public void Initialize()
        {
            _pickUpComponent.Init(_headCamera);
        }

        public void ChangeMovementActive(bool active)
        {
            _movementController.SetActive(active);
        }

        private void Update()
        {
            if (_cursorEnabled) {
                _pickUpComponent.Tick();
            }

            if (_inventoryEnabled) {
                // todo neiran take to another component
                if (Input.GetKeyDown(KeyCode.I)) {
                    _inventoryStatusPublisher.Publish(InventoryStatusEvent.INVENTORY_CHANGED, new());
                }
            }
        }

        public void SetPosition(PlayerSpawnPoint spawnPoint)
        {
            SetPosition(spawnPoint.Position);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}