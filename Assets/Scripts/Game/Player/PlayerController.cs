using Game.Tools.Component;
using Game.Utils;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        private float _raycastLength = 1.5f;
        private bool _cursorEnabled = true;

        private Transform _head = null!;

        private LayerMask _layerMask;

        private void Awake()
        {
            _head = this.RequireComponentInChildren<Transform>("Head");
            _layerMask = LayerMask.GetMask("Tool");
        }

        private void Update()
        {
            if (!_cursorEnabled) {
                return;
            }

            bool result = Physics.Raycast(_head.position, _head.forward, out RaycastHit hit, _raycastLength, _layerMask);
            if (!result) {
                return;
            }

            ToolController toolController = hit.transform.GetComponent<ToolController>();
            if (toolController == null) {
                return;
            }

            Debug.Log("Watching at a tool now. Wanna press E");
            // todo neiran add to inventory
        }
    }
}