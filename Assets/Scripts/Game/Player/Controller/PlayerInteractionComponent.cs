using System;
using Game.Common.Controller;
using UnityEngine;

namespace Game.Player.Controller
{
    public class PlayerInteractionComponent : MonoBehaviour
    {
        public float _raycastLength = 2.75f;

        private Transform _head = null!;
        private LayerMask _lookLayerMask;

        public event Action? OnLook;
        public event Action? OnUnlook;
        public event Action<IInteractableComponent>? OnInteractStarted;
        public event Action<IInteractableComponent>? OnInteractFinished;

        private IInteractableComponent? _cachedInteractionLook;
        private IInteractableComponent? _currentLook;
        
        private bool _isInteracting;

        public void Init(Transform head)
        {
            _head = head;
            _lookLayerMask = LayerMask.GetMask("Tool", "Tile", "Plant", "Harvest"); // todo neiran to combined layer mask to workaround or make descriptor
        }

        public void Tick()
        {
            RunLook();
            RunInteractionStart();
            RunInteractionEnd();
        }

        private void RunLook()
        {
            bool result = Physics.Raycast(_head.position, _head.forward, out RaycastHit hit, _raycastLength, _lookLayerMask);
            if (!result) {
                TryResetLook();
                return;
            }

            Transform currentHit = hit.transform;
            IInteractableComponent interactableComponent = currentHit.GetComponentInParent<IInteractableComponent>();
            if (interactableComponent == null) {
                TryResetLook();
                return;
            }

            if (_currentLook == interactableComponent) {
                return;
            }

            _currentLook = interactableComponent;
            OnLook?.Invoke();
        }

        private void RunInteractionStart()
        {
            if (_currentLook == null) {
                return;
            }

            if (_isInteracting) {
                // todo neiran maybe we will need interaction every frame. But we can use InteractionPressed. Think about it if needed
                return;
            }

            if (!Input.GetKeyDown(KeyCode.E)) {
                return;
            }

            _cachedInteractionLook = _currentLook;
            OnInteractStarted?.Invoke(_currentLook);
            _isInteracting = true;
        }

        private void RunInteractionEnd()
        {
            if (_cachedInteractionLook == null) {
                return;
            }

            if (_currentLook != null && !Input.GetKeyUp(KeyCode.E)) {
                return;
            }
            
            _isInteracting = false;
            OnInteractFinished?.Invoke(_cachedInteractionLook);
            _cachedInteractionLook = null;

        }

        private void TryResetLook()
        {
            if (_currentLook == null) {
                return;
            }

            OnUnlook?.Invoke();
            _currentLook = null;
        }
        
        public IInteractableComponent? CurrentLook => _currentLook;

        public bool InteractionPressed => _isInteracting;
    }
}