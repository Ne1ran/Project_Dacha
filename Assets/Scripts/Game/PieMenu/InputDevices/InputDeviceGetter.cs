using System;
using Game.PieMenu.Model;
using Game.PieMenu.Utils;
using UnityEngine;

namespace Game.PieMenu.InputDevices
{
    public class InputDeviceGetter
    {
        public IInputDevice InputDevice { get; private set; } = null!;
        private bool _isOldInputSystemEnabled;
        private bool _isNewInputSystemEnabled;

        public void Initialize(GameObject pieMenuGO)
        {
            DetectInputSystem();
            HandleInputDevicePreferences(pieMenuGO);
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

        private void HandleInputDevicePreferences(GameObject obj)
        {
            int defaultValue = -1;
            int inputDeviceId = PlayerPrefs.GetInt(PieMenuUtils.InputDevice, defaultValue);

            if (_isOldInputSystemEnabled) {
                if (inputDeviceId == (int) AvailableInputDevices.MouseAndKeyboardOldInputSystem) {
                    SetInputDevice<PieMenuOldInputSystem>(obj);
                } else {
                    SetDefault(obj);
                }
            } else if (_isNewInputSystemEnabled) {
                if (inputDeviceId == (int) AvailableInputDevices.MouseAndKeyboardNewInputSystem) {
                    SetInputDevice<PieMenuNewInputSystem>(obj);
                } else {
                    SetDefault(obj);
                }
            }
        }

        private void SetDefault(GameObject obj)
        {
            if (_isOldInputSystemEnabled) {
                SetInputDevice<PieMenuOldInputSystem>(obj);
            } else if (_isNewInputSystemEnabled) {
                SetInputDevice<PieMenuNewInputSystem>(obj);
            }
        }

        private void SetInputDevice<T>(GameObject obj)
                where T : MonoBehaviour, IInputDevice
        {
            if (typeof(IInputDevice).IsAssignableFrom(typeof(T))) {
                InputDevice = obj.AddComponent<T>();
            } else {
                throw new InvalidOperationException($"Type {typeof(T)} must derive from MonoBehaviour and implement IInputDevice interface.");
            }
        }
    }
}