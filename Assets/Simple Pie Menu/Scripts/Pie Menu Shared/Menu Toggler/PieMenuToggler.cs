using System.Collections;
using System.Collections.Generic;
using Simple_Pie_Menu.Scripts.Pie_Menu;
using Simple_Pie_Menu.Scripts.Pie_Menu.Menu_Item_Selection;
using Simple_Pie_Menu.Scripts.Pie_Menu.References;
using Simple_Pie_Menu.Scripts.Pie_Menu.Settings;
using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Pie_Menu_Shared.Menu_Toggler
{
    public class PieMenuToggler : MonoBehaviour
    {
        private PieMenuGeneralSettings _generalSettings = null!;

        private readonly Dictionary<PieMenuMain, PieMenuState> _pieMenuStates = new();
        private PieMenuState _currentMenu = null!;

        private int _openedPieMenusCount;

        private void Update()
        {
            CloseOnReturnPressed();
        }

        public void SetActive(PieMenuMain pieMenu, bool isActive)
        {
            RegisterNewMenu(pieMenu);

            PieMenuState? state = _pieMenuStates[pieMenu];
            DisableInfoPanel(state);
            if (isActive) {
                ShowPieMenu(state);
            } else {
                HidePieMenu(state);
            }
        }

        private void RegisterNewMenu(PieMenuMain pieMenu)
        {
            if (_pieMenuStates.ContainsKey(pieMenu)) {
                return;
            }

            _generalSettings = pieMenu.PieMenuSettingsModel.GeneralSettings;

            PieMenuState newState = new();
            newState.SetComponents(pieMenu);
            _pieMenuStates[pieMenu] = newState;
        }

        private void ShowPieMenu(PieMenuState state)
        {
            state.PieMenuModel.SetTransitionState(true);
            state.PieMenuGo.SetActive(true);
            _generalSettings.PlayAnimation(state.Animator, PieMenuGeneralSettings.TriggerActiveTrue);
            StartCoroutine(WaitForAudioAndAnimationToFinishPlaiyng(state, true));
        }

        private void HidePieMenu(PieMenuState state)
        {
            state.Selector.ToggleSelection(false);
            _generalSettings.PlayAnimation(state.Animator, PieMenuGeneralSettings.TriggerActiveFalse);
            StartCoroutine(WaitForAudioAndAnimationToFinishPlaiyng(state, false));
        }

        private IEnumerator WaitForAudioAndAnimationToFinishPlaiyng(PieMenuState state, bool isActive)
        {
            float timeToWait = CalculateTimeToWait(state.PieMenuMain);

            yield return new WaitForSeconds(timeToWait);

            if (isActive) {
                EnableInfoPanel(state);
                state.Selector.ToggleSelection(true);
                state.Selector.EnableClickDetecting();

                ManageScriptLifecycle(true, state);
            } else {
                state.PieMenuGo.SetActive(false);
                state.PieMenuModel.SetTransitionState(false);
                ManageScriptLifecycle(false);
            }

            state.PieMenuMain.PieMenuModel.SetActiveState(isActive);
        }

        private float CalculateTimeToWait(PieMenuMain pieMenu)
        {
            PieMenuModel pieMenuModel = pieMenu.PieMenuModel;
            float audioClipLength = 0f;
            if (pieMenuModel.MouseClick != null) {
                audioClipLength = pieMenuModel.MouseClick.length;
            }

            float animationClipLength = 0f;
            if (pieMenuModel.Animation != null) {
                animationClipLength = pieMenuModel.Animation.length;
            }

            return Mathf.Max(audioClipLength, animationClipLength);
        }

        private void ManageScriptLifecycle(bool isMenuOpening, PieMenuState state = null)
        {
            if (isMenuOpening) {
                _currentMenu = state;
                enabled = true;
                _openedPieMenusCount++;
            } else {
                _openedPieMenusCount--;
                if (_openedPieMenusCount != 0) {
                    return;
                }
                _currentMenu = null;
                enabled = false;
            }
        }

        private void CloseOnReturnPressed()
        {
            if (_currentMenu == null) {
                return;
            }
            bool closeable = _currentMenu.PieMenuModel.IsCloseable;
            if (!closeable) {
                return;
            }

            bool returnPressed = _currentMenu.Selector.InputDeviceGetter.InputDevice.IsCloseButtonPressed();
            if (returnPressed) {
                SetActive(_currentMenu.PieMenuMain, false);
            }
        }

        private void DisableInfoPanel(PieMenuState state)
        {
            if (state.PieMenuModel.InfoPanelEnabled) {
                _generalSettings.SetActive(state.PieMenuMain, false);
            }
        }

        private void EnableInfoPanel(PieMenuState state)
        {
            if (!state.PieMenuModel.InfoPanelEnabled) {
                return;
            }
            PieMenuMain pieMenu = state.PieMenuMain;
            _generalSettings.SetActive(pieMenu, true);
            _generalSettings.ModifyHeader(pieMenu, string.Empty);
            _generalSettings.ModifyDetails(pieMenu, string.Empty);
        }
    }

    internal class PieMenuState
    {
        public PieMenuMain PieMenuMain { get; private set; } = null!;
        public PieMenuModel PieMenuModel { get; private set; } = null!;
        public PieMenuElements PieMenuElements { get; private set; } = null!;
        public GameObject PieMenuGo { get; private set; } = null!;
        public MenuItemSelector Selector { get; private set; } = null!;
        public Animator Animator { get; private set; } = null!;

        public void SetComponents(PieMenuMain pieMenu)
        {
            PieMenuMain = pieMenu;
            PieMenuModel = pieMenu.PieMenuModel;
            PieMenuElements = pieMenu.PieMenuElements;
            PieMenuGo = pieMenu.gameObject;
            Selector = pieMenu.PieMenuSettingsModel.MenuItemSelector;
            Animator = PieMenuElements.Animator;
        }
    }
}