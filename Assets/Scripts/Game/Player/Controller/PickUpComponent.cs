using System;
using Game.Common.Controller;
using Game.Tools.Component;
using JetBrains.Annotations;
using UnityEngine;

namespace Game.Player.Controller
{
    public class PickUpComponent : MonoBehaviour
    {
        private float _raycastLength = 1.5f;

        private Transform _head = null!;
        private LayerMask _layerMask;

        public event Action OnLook;
        public event Action OnUnlook;
        public event Action<IInteractableComponent> OnInteract;

        [CanBeNull]
        private IInteractableComponent _currentLook;

        public void Init(Transform head)
        {
            _head = head;
            _layerMask = LayerMask.GetMask("Tool"); // todo neiran to combined layer mask to workaround
        }

        public void Tick()
        {
            RunLook();
            RunPickUp();
        }

        private void RunLook()
        {
            bool result = Physics.Raycast(_head.position, _head.forward, out RaycastHit hit, _raycastLength, _layerMask);
            if (!result) {
                TryResetLook();
                return;
            }

            Transform currentHit = hit.transform;
            if (!currentHit.TryGetComponent(out IInteractableComponent lookInteractionObj)) {
                TryResetLook();
                return;
            }

            if (_currentLook == lookInteractionObj) {
                return;
            }

            _currentLook = lookInteractionObj;
            OnLook?.Invoke();
        }

        private void RunPickUp()
        {
            if (_currentLook == null) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                OnInteract?.Invoke(_currentLook);
            }
        }

        private void TryResetLook()
        {
            if (_currentLook == null) {
                return;
            }

            OnUnlook?.Invoke();
            _currentLook = null;
        }
    }
}