using SimplePieMenu;
using UnityEngine;

namespace Game.PieMenu.InputDevices
{
    public class PieMenuNewInputSystem : MonoBehaviour, IInputDevice
    {
        private PieMenuControls _pieMenuControls = null!;

        private Vector2 _cursorPosition;
        private bool _isSelectionCanceled;
        private bool _isReturnCanceled;

        private void Awake()
        {
            _pieMenuControls = new();

            _pieMenuControls.MouseAndKeyboard.PointerPosition.performed += ctx => OnCursorPositionPerformed(ctx.ReadValue<Vector2>());

            //canceled event is triggered when the button is released
            _pieMenuControls.MouseAndKeyboard.Selection.canceled += _ => OnSelectionCanceled();
            _pieMenuControls.MouseAndKeyboard.Close.canceled += _ => OnCloseCanceled();

            _pieMenuControls.Enable();
        }

        private void OnDestroy()
        {
            _pieMenuControls.Disable();
        }

        public Vector2 GetPosition(Vector2 anchoredPosition)
        {
            Vector2 position;

            position.x = _cursorPosition.x - (Screen.width / 2f) - anchoredPosition.x;
            position.y = _cursorPosition.y - (Screen.height / 2f) - anchoredPosition.y;

            return position;
        }

        public bool IsSelectionButtonPressed()
        {
            return IsButtonPressed(ref _isSelectionCanceled);
        }

        public bool IsCloseButtonPressed()
        {
            return IsButtonPressed(ref _isReturnCanceled);
        }

        private bool IsButtonPressed(ref bool buttonCanceled)
        {
            if (!buttonCanceled) {
                return false;
            }

            buttonCanceled = false;
            return true;
        }

        private void OnCursorPositionPerformed(Vector2 pointerPosition)
        {
            _cursorPosition = pointerPosition;
        }

        private void OnSelectionCanceled()
        {
            _isSelectionCanceled = true;
        }

        private void OnCloseCanceled()
        {
            _isReturnCanceled = true;
        }
    }
}