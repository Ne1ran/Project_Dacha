using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Interactable.ViewModel;
using Game.PieMenu.InputDevices;
using Game.PieMenu.Model;
using Game.PieMenu.UI.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.PieMenu.UI.Common
{
    public class MenuItemSelector : MonoBehaviour
    {
        private const int CircleDegrees = 360;

        private const float CheckDelay = 0.1f;

        public InputDeviceGetter InputDeviceGetter { get; private set; } = null!;

        public bool SelectionEnabled { get; private set; }
        public bool CanBeClicked { get; private set; }

        private int _selection;
        private bool _isChecking;

        private PieMenuController _pieMenu = null!;

        public Dictionary<int, PieMenuItemController> PieMenuItemsReference { get; private set; } = null!;

        private int _previousSelection;

        private bool _selectionConstrained;
        private int _constraintMaxDistance;

        public void ResetPreviousSelection()
        {
            _previousSelection = -1;
        }

        public void RegisterMenuItems(Dictionary<int, PieMenuItemController> pieMenuItems)
        {
            PieMenuItemsReference = pieMenuItems;
        }

        public void SelectMenuItem(int selection)
        {
            if (selection == _previousSelection) {
                return;
            }

            if (_previousSelection == 0) {
                return;
                // todo neiran check this
            }

            UnselectPreviousMenuItem();
            PieMenuItemsReference[selection].OnPointerEnter();
            _previousSelection = selection;
        }

        public void UnselectPreviousMenuItem()
        {
            if (_previousSelection == -1) {
                return;
            }

            if (_previousSelection == 0) {
                return;
                // todo neiran check this
            }

            PieMenuItemsReference[_previousSelection].BeforePointerExit();
            PieMenuItemsReference[_previousSelection].OnPointerExit();
            EventSystem.current.SetSelectedGameObject(null);

            _previousSelection = -1;
        }

        public void ToggleSelectionConstraint(bool selectionConstrain)
        {
            _selectionConstrained = selectionConstrain;
        }

        public void SetConstraintMaxDistance(int maxDistance)
        {
            _constraintMaxDistance = maxDistance;
        }

        public int CalculateSelection(PieMenuController pieMenu, IInputDevice inputDevice)
        {
            int selection;

            // This method calculates the angle from 0 to 360 degrees based on the mouse position relative to the center of the pie menu.
            float currentAngle = CalculateAngle(inputDevice, pieMenu.PieMenuModel.AnchoredPosition);
            if (!Mathf.Approximately(currentAngle, -1)) {
                // This method takes into account the pie menu rotation to adjust currentAngle
                float adjustedAngle = AdjustAngle(pieMenu, currentAngle);

                int menuItemCount = pieMenu.ViewModel.PieMenuItems.Count;
                selection = (int) adjustedAngle / (CircleDegrees / menuItemCount);

                // This method adjusts the Menu Item selection taking into account their spacing.
                selection = AdjustSelection(pieMenu, selection, currentAngle);

                //This calculation reverses the selection direction. Without it, the items would be selected clockwise,
                //starting from the left-center of the circle(in our case, the items should be selected in the opposite direction).
                selection = menuItemCount - selection;

                //For a 360 degree angle, the calculation result is incorrect, and it should be adjusted like this:
                if (selection >= menuItemCount) {
                    selection = 0;
                }
            } else {
                selection = -1;
            }

            return selection;
        }

        private float CalculateAngle(IInputDevice inputDevice, Vector2 anchoredPosition)
        {
            Vector2 inputOnScreenPosition = inputDevice.GetPosition(anchoredPosition);
            if (_selectionConstrained) {
                float distance = inputOnScreenPosition.magnitude;
                if (distance < _constraintMaxDistance) {
                    return -1;
                }
            }

            float currentAngle = Mathf.Atan2(inputOnScreenPosition.y, -inputOnScreenPosition.x) * Mathf.Rad2Deg;
            currentAngle = (currentAngle + CircleDegrees) % CircleDegrees;
            return currentAngle;
        }

        private float AdjustAngle(PieMenuController pieMenu, float currentAngle)
        {
            PieMenuModel pieMenuModel = pieMenu.PieMenuModel;
            float adjustedAngle = (currentAngle + pieMenuModel.Rotation + CircleDegrees) % CircleDegrees;
            return adjustedAngle;
        }

        private int AdjustSelection(PieMenuController pieMenu, int selection, float currentAngle)
        {
            PieMenuModel pieMenuModel = pieMenu.PieMenuModel;
            float menuItemSpacing = pieMenuModel.MenuItemSpacing;
            int minSpacing = 1;
            if (menuItemSpacing <= minSpacing) {
                return selection;
            }

            // Calculate the start and end angles of the current Menu Item, considering spacing and rotation.
            float menuItemDegrees = pieMenuModel.MenuItemDegrees;
            float menuItemStartAngle = (menuItemDegrees + menuItemSpacing) * selection;
            float menuItemEndAngle = menuItemStartAngle + menuItemDegrees;

            float halfOfTheMenuItemSpacing = menuItemSpacing / 2;
            menuItemStartAngle -= halfOfTheMenuItemSpacing;
            menuItemEndAngle += halfOfTheMenuItemSpacing;

            int pieMenuRotation = pieMenuModel.Rotation;
            menuItemStartAngle += CircleDegrees - pieMenuRotation;
            menuItemEndAngle += CircleDegrees - pieMenuRotation;

            menuItemStartAngle = (menuItemStartAngle + CircleDegrees) % CircleDegrees;
            menuItemEndAngle = (menuItemEndAngle + CircleDegrees) % CircleDegrees;

            // Checking if the current angle is within the calculated range
            if ((menuItemEndAngle < menuItemStartAngle && (currentAngle >= menuItemStartAngle || currentAngle <= menuItemEndAngle))
                || (currentAngle >= menuItemStartAngle && currentAngle <= menuItemEndAngle)) {
                // No adjustment needed
            } else {
                // Adjusting selection and handling wrap-around
                selection++;
                int firstIndex = 0;
                int menuItemsCount = pieMenu.ViewModel.PieMenuItems.Count;
                if (selection >= menuItemsCount) {
                    selection = firstIndex;
                }
            }

            return selection; // No adjustment needed
        }

        public void Initialize(PieMenuController pieMenu, PieMenuSettingsModel settingsModel)
        {
            _pieMenu = pieMenu;
            InputDeviceGetter = settingsModel.InputDeviceGetter;
            SaveReferencesToMenuItems();
        }

        private void Update()
        {
            if (SelectionEnabled) {
                if (!_isChecking) {
                    TryHandleSelectionAsync().Forget();
                }

                DetectClick();
            } else {
                UnselectPreviousMenuItem();
            }
        }

        public void ToggleSelection(bool isEnabled)
        {
            SelectionEnabled = isEnabled;
            if (isEnabled) {
                ResetPreviousSelection();
            }
        }

        public void EnableClickDetecting()
        {
            CanBeClicked = true;
        }

        private async UniTaskVoid TryHandleSelectionAsync()
        {
            _isChecking = true;
            await UniTask.WaitForSeconds(CheckDelay, cancellationToken: destroyCancellationToken);

            _selection = CalculateSelection(_pieMenu, InputDeviceGetter.InputDevice);

            if (_selection != -1) {
                SelectMenuItem(_selection);
            } else {
                UnselectPreviousMenuItem();
            }

            _isChecking = false;
        }

        private void DetectClick()
        {
            if (!InputDeviceGetter.InputDevice.IsSelectionButtonPressed() || !CanBeClicked || _selection == -1) {
                return;
            }

            bool menuItemDisabled = !PieMenuItemsReference[_selection].Interactable;
            if (menuItemDisabled) {
                return;
            }

            CanBeClicked = false;
            PieMenuItemsReference[_selection].OnClick();
        }

        private void SaveReferencesToMenuItems()
        {
            RegisterMenuItems(_pieMenu.ViewModel.PieMenuItems);
        }
    }
}