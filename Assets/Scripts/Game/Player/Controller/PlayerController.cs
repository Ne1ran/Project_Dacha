using Core.Resources.Binding.Attributes;
using Cysharp.Threading.Tasks;
using Game.Common.Controller;
using Game.Equipment.Component;
using Game.Inventory.Event;
using Game.Items.Controller;
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
        [ComponentBinding("RightHand")]
        private Transform _rightHand = null!;
        [ComponentBinding("LeftHand")]
        private Transform _leftHand = null!;
        
        [Inject]
        private PlayModeService _playModeService = null!;
        [Inject]
        private IPublisher<string, InventoryStatusEvent> _inventoryStatusPublisher = null!;

        private PickUpComponent _pickUpComponent = null!;
        private MovementController _movementController = null!;
        private EquipmentController _equipmentController = null!;
        private Transform _headCamera = null!;

        private bool _cursorEnabled = true;
        private bool _inventoryEnabled = true;

        private void Awake()
        {
            _headCamera = this.RequireComponentInChildren<Camera>().transform;
            _pickUpComponent = this.AddComponent<PickUpComponent>();
            _movementController = this.RequireComponent<MovementController>();
            _equipmentController = this.RequireComponent<EquipmentController>();

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
            _equipmentController.Init(_rightHand, _leftHand);
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
    }
}