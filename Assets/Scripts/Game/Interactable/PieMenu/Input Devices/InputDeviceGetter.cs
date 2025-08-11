using System;
using Game.Interactable.PieMenu.Model;
using Game.Interactable.PieMenu.Others;
using UnityEngine;

namespace Game.Interactable.PieMenu.Input_Devices
{
    public class InputDeviceGetter : MonoBehaviour
    {
        public IInputDevice InputDevice { get; private set; } = null!;
        private bool _isOldInputSystemEnabled;
        private bool _isNewInputSystemEnabled;

        private void Awake()
        {
            DetectInputSystem();
            HandleInputDevicePreferences();
        }

        private void DetectInputSystem()
        {
#if ENABLE_INPUT_SYSTEM
            // New input system backends are enabled.
            _isNewInputSystemEnabled = true;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            // Old input backends are enabled.
            _isOldInputSystemEnabled = true;
#endif
        }

        private void HandleInputDevicePreferences()
        {
            int defaultValue = -1;
            int inputDeviceId = PlayerPrefs.GetInt(PieMenuPlayerPrefs.InputDevice, defaultValue);

            if (_isOldInputSystemEnabled) {
                if (inputDeviceId == (int) AvailableInputDevices.MouseAndKeyboard_OLD_INPUT_SYSTEM) {
                    SetInputDevice<PieMenuOldInputSystem>();
                } else {
                    SetDefault();
                }
            } else if (_isNewInputSystemEnabled) {
                if (inputDeviceId == (int) AvailableInputDevices.MouseAndKeyboard_NEW_INPUT_SYSTEM) {
                    SetInputDevice<PieMenuNewInputSystem>();
                } else {
                    SetDefault();
                }
            }
        }

        private void SetDefault()
        {
            if (_isOldInputSystemEnabled) {
                SetInputDevice<PieMenuOldInputSystem>();
            } else if (_isNewInputSystemEnabled) {
                SetInputDevice<PieMenuNewInputSystem>();
            }
        }

        private void SetInputDevice<T>()
                where T : MonoBehaviour, IInputDevice
        {
            if (typeof(IInputDevice).IsAssignableFrom(typeof(T))) {
                InputDevice = gameObject.AddComponent<T>();
            } else {
                throw new InvalidOperationException($"Type {typeof(T)} must derive from MonoBehaviour and implement IInputDevice interface.");
            }
        }
    }
}