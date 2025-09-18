using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Equipment.Component;
using Game.GameMap.Spawn;
using Game.Inventory.Event;
using Game.Items.Controller;
using Game.Movement;
using Game.PlayMode.Service;
using Game.Utils;
using MessagePipe;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace Game.Player.Controller
{
    [NeedBinding("Prefabs/Player/Player")]
    public class PlayerController : MonoBehaviour
    {
        [ComponentBinding("RightHand")]
        private Transform _rightHand = null!;
        [ComponentBinding("LeftHand")]
        private Transform _leftHand = null!;
        
        [Inject]
        private PlayModeService _playModeService = null!;
        [Inject]
        private IPublisher<string, InventoryStatusEvent> _inventoryStatusPublisher = null!;

        private PlayerInteractionComponent _playerInteractionComponent = null!;
        private MovementController _movementController = null!;
        private EquipmentController _equipmentController = null!;
        private Transform _headCamera = null!;

        private bool _cursorEnabled = true;
        private bool _inventoryEnabled = true;

        private void Awake()
        {
            _headCamera = this.RequireComponentInChildren<Camera>().transform;
            _playerInteractionComponent = this.AddComponent<PlayerInteractionComponent>();
            _movementController = this.RequireComponent<MovementController>();
            _equipmentController = this.RequireComponent<EquipmentController>();

            _playerInteractionComponent.OnLook += OnPlayerInteractionStarted;
            _playerInteractionComponent.OnUnlook += OnPlayerInteractionFinished;
            _playerInteractionComponent.OnInteractStarted += OnInteractStarted;
            _playerInteractionComponent.OnInteractFinished += OnInteractFinished;
        }

        private void OnDestroy()
        {
            _playerInteractionComponent.OnLook -= OnPlayerInteractionStarted;
            _playerInteractionComponent.OnUnlook -= OnPlayerInteractionFinished;
            _playerInteractionComponent.OnInteractStarted -= OnInteractStarted;
            _playerInteractionComponent.OnInteractFinished -= OnInteractFinished;
        }

        private void OnPlayerInteractionStarted()
        {
            _playModeService.PlayModeScreen.ShowCrosshair(true);
        }

        private void OnPlayerInteractionFinished()
        {
            _playModeService.PlayModeScreen.FadeCrosshair(true);
        }

        private void OnInteractStarted(IInteractableComponent target)
        {
            target.Interact().Forget();
        }

        private void OnInteractFinished(IInteractableComponent target)
        {
            target.StopInteract().Forget();
        }

        public void Initialize()
        {
            _playerInteractionComponent.Init(_headCamera);
            _equipmentController.Init(_rightHand, _leftHand);
        }

        public void ChangeMovementActive(bool active)
        {
            _movementController.ChangeMoveActive(active);
        }

        public void ChangeLookActive(bool active)
        {
            _movementController.ChangeLookActive(active);
        }

        private void Update()
        {
            if (_cursorEnabled) {
                _playerInteractionComponent.Tick();
            }

            if (!_inventoryEnabled) {
                return;
            }

            // todo neiran take to another component
            if (Input.GetKeyDown(KeyCode.I)) {
                _inventoryStatusPublisher.Publish(InventoryStatusEvent.INVENTORY_CHANGED, new());
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

        public void EquipItem(ItemController item)
        {
            // todo neiran change to diff hands through enum right-left-both-none? to equip.
            _equipmentController.EquipItemRightHand(item.transform);
        }

        public void UnequipItem()
        {
            _equipmentController.ClearEquipment();
        }

        public Vector3 Forward => _movementController.Forward;

        public bool InteractionButtonPressed => _playerInteractionComponent.InteractionPressed;
    }
}