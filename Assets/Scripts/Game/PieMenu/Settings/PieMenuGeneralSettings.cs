using System.Collections.Generic;
using Game.PieMenu.Model;
using Game.PieMenu.UI;
using Game.PieMenu.UI.Common;
using Game.PieMenu.UI.Model;
using Game.PieMenu.Utils;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PieMenuItemController = Game.PieMenu.UI.PieMenuItemController;

namespace Game.PieMenu.Settings
{
    public class PieMenuGeneralSettings : MonoBehaviour
    {
        public const string TriggerActiveTrue = "ActiveTrue";
        public const string TriggerActiveFalse = "ActiveFalse";
        public const float MaxInfoPanelScale = 2f;

        [SerializeField]
        private bool _infoPanelEnabled;

        [SerializeField]
        private Color _headerColor;

        [SerializeField]
        private Color _detailsColor;

        [Range(0f, MaxInfoPanelScale)]
        [SerializeField]
        private float _scale;

        [SerializeField]
        private AvailableInputDevices _inputDevice;

        [Header("Left/Right")]
        [Range(-1000, 1000)]
        [SerializeField]
        private int _horizontalPositionOffset;

        [Header("Up/Down")]
        [Range(1000, -1000)]
        [SerializeField]
        private int _verticalPositionOffset;

        [Tooltip("This option disables selection in the center of your Pie Menu. "
                 + "This can be especially useful when you have a large number of options, "
                 + "where even slight mouse movements can easily change the selection. ")]
        [SerializeField]
        private bool _selectionConstrained;

        [Range(0, 100)]
        [SerializeField]
        private int _menuItemSpacing;

        [Range(0, 360)]
        [SerializeField]
        private int _rotation;

        [Range(250, 1000)]
        [SerializeField]
        private int _size;

        [Range(-500f, 0f)]
        [SerializeField]
        private int _offsetFromCenter;

        [SerializeField]
        private Color _normalColor;

        [SerializeField]
        private Color _selectedColor;

        [SerializeField]
        private Color _disabledColor;

        [SerializeField]
        private List<AnimatorOverrideController> _animatorOverrideControllers = new();
        
        [SerializeField]
        private Image _background = null!;

        [SerializeField]
        private Transform _infoPanel = null!;

        [SerializeField]
        private TextMeshProUGUI _header = null!;

        [SerializeField]
        private TextMeshProUGUI _details = null!;

        [SerializeField]
        private Animator _animator = null!;

        [SerializeField]
        private AudioSource _mouseClickAudioSource = null!;

        public Image Background => _background;

        public Transform InfoPanel => _infoPanel;

        public TextMeshProUGUI Header => _header;

        public TextMeshProUGUI Details => _details;

        public Animator Animator => _animator;

        public AudioSource MouseClickAudioSource => _mouseClickAudioSource;

        private PieMenuController _pieMenuController = null!;
        private PieMenuModel _pieMenuModel = null!;

        public List<AnimatorOverrideController> AnimatorOverrideControllers => _animatorOverrideControllers;

        public int AnimationDropdownList { get; private set; }
        public List<string> AnimationNames { get; private set; } = new();

        private AnimationClip _clip = null!;
        private double _previewStartTime;

        public void Initialize(PieMenuController pieMenuController)
        {
            _pieMenuController = pieMenuController;
            _pieMenuModel = pieMenuController.PieMenuSettingsModel.PieMenuModel;
            
            InitSizeAndSpacing();
            ConstraintSelection(_selectionConstrained);
            InitializeAnimationsSettings();
            InitializeInfoPanelSettings();
        }

        private void InitSizeAndSpacing()
        {
            _size = _pieMenuModel.MenuItemSize;
            _pieMenuModel.SetSpacing(_menuItemSpacing);
        }

        private void RestoreRotationToDefault()
        {
            _rotation = 0;
            ChangeRotation(_rotation);
        }

        public void UpdateButtons(int menuItemCount, int menuItemSpacing)
        {
            if (menuItemCount <= 0) {
                return;
            }

            _pieMenuModel.SetSpacing(menuItemSpacing);
            UpdateFillAmount(menuItemCount, menuItemSpacing);
            _pieMenuModel.SetMenuItemAngle(PieMenuUtils.CalculateItemAngle(_pieMenuController));
        }

        public void ChangeRotation(int rotation)
        {
            _pieMenuModel.SetRotation(rotation);
        }

        public void UpdateFillAmount(int menuItemCount, int menuItemSpacing)
        {
            float fillAmount = CalculateMenuItemFillAmount(menuItemCount, menuItemSpacing);

            _pieMenuModel.SetFillAmount(fillAmount);
            ModifyFillAmount(_pieMenuController.GetMenuItems(), fillAmount, menuItemSpacing);
        }

        public void ModifyFillAmount(Dictionary<int, PieMenuItemController> menuItems, float fillAmount, float menuItemSpacing)
        {
            int iteration = 0;
            foreach (KeyValuePair<int, PieMenuItemController> item in menuItems) {
                Image image = item.Value.GetComponent<Image>();
                image.fillAmount = fillAmount;

                float zAxisRotation = (fillAmount * iteration * PieMenuUtils.CircleDegrees_F) + (menuItemSpacing * iteration);
                item.Value.transform.rotation = Quaternion.Euler(0, 0, zAxisRotation);
                iteration++;
            }
        }

        public float CalculateMenuItemFillAmount(int menuItemCount, int menuItemSpacing)
        {
            float totalSpacingPercentage = PieMenuUtils.CalculateTotalSpacingPercentage(menuItemCount, menuItemSpacing);

            float maxFillAmount = 1f;
            float fillAmountLeft = maxFillAmount - totalSpacingPercentage;
            float fillAmount = fillAmountLeft / menuItemCount;
            return fillAmount;
        }

        // todo neiran dont forget
        private void ChangeMenuItemsColors(ColorBlock newColors)
        {
            foreach (PieMenuItemController pieMenuItem in _pieMenuController.ViewModel.PieMenuItems.Values) {
                pieMenuItem.ChangeColor(newColors);
            }
        }

        public void SetOffset(int newOffset)
        {
            _offsetFromCenter = newOffset;
        }

        public void Rotate(Transform menuItem, Quaternion rotation)
        {
            int iconDirIndex = 0;
            Transform iconDir = menuItem.GetChild(iconDirIndex);

            Quaternion parentRotation = menuItem.rotation;
            Quaternion newRotation = parentRotation * rotation;

            iconDir.rotation = newRotation;
        }

        public void Rotate(Dictionary<int, PieMenuItemController> menuItems, Quaternion iconDirRotation)
        {
            //The first element in the list of icons requires a different mathematical pattern than the rest.
            PieMenuUtils.RotateFirstElement(menuItems[0].transform, iconDirRotation);
            PieMenuUtils.RotateOtherElements(menuItems, iconDirRotation);
        }
        
        public void Resize(float newScale)
        {
            if (!_pieMenuModel.InfoPanelEnabled) {
                return;
            }

            ChangeInfoPanelScale(newScale);
            SetScale(newScale);
        }

        // todo neiran dont forget to redo or pick
        public void MoveIcon(Transform icon, int offsetFromCenter)
        {
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector3(offsetFromCenter, 0f, 0f);
        }

        public int GetInitialOffset(Transform iconDir)
        {
            Transform icon = iconDir.GetChild(0);
            int currentOffset = (int) icon.localPosition.x;
            return currentOffset;
        }

        private int CalculateNewOffset(float initialOffset)
        {
            int initialSize = _pieMenuModel.MenuItemInitialSize;
            int currentSize = _pieMenuModel.MenuItemSize;

            float newOffset = (initialOffset / initialSize) * currentSize;
            return Mathf.RoundToInt(newOffset);
        }

        public void SwapAnimations()
        {
            SwapAnimationControllers(AnimationDropdownList);
        }

        private void InitializeAnimationsSettings()
        {
            InitializeList();
            GetSelectedAnimation();
        }

        private void InitializeList()
        {
            AnimationNames = new();
            foreach (AnimatorOverrideController overrideController in AnimatorOverrideControllers) {
                AnimationClip animationClip = GetAnimation(overrideController);
                AnimationNames.Add(animationClip.name);
            }
        }

        private void GetSelectedAnimation()
        {
            RuntimeAnimatorController runtimeController = _animator.runtimeAnimatorController;
            AnimationClip animationClip = GetAnimation(runtimeController);
            string currentAnimationName = animationClip.name;

            //if the List<T>.FindIndex method doesn't find a matching element, it returns -1
            int index = AnimationNames.FindIndex(animationName => animationName == currentAnimationName);

            if (index == -1) {
                throw new("It's likely that you've added a new animation to your menu incorrectly."
                          + " Please refer to the documentation to learn more.");
            }

            AnimationDropdownList = index;
            _pieMenuModel.SetAnimation(animationClip);
        }

        public void SwapAnimationControllers(int controllerIndex)
        {
            AnimatorOverrideController overrideController = GetAnimator(controllerIndex);
            Animator.runtimeAnimatorController = overrideController;
            _pieMenuModel.SetAnimation(GetAnimation(overrideController));
        }

        public AnimatorOverrideController GetAnimator(int controllerIndex)
        {
            return _animatorOverrideControllers[controllerIndex];
        }

        public AnimationClip GetAnimation(RuntimeAnimatorController runtimeController)
        {
            return runtimeController.animationClips[0];
        }

        public void PlayAnimation(string trigger)
        {
            _animator.SetTrigger(trigger);
        }

        public void SetScale(float scale)
        {
            _scale = scale;
        }

        private void InitializeInfoPanelSettings()
        {
            _pieMenuModel.SetInfoPanelEnabled(_infoPanelEnabled);
        }
        
        public void HandleEnableValueChange(bool isEnabled)
        {
            _pieMenuModel.SetInfoPanelEnabled(isEnabled);
            Resize(_pieMenuModel.Scale);
            SetActiveInfoPanel(isEnabled);
        }

        public void SetActiveInfoPanel(bool isActive)
        {
            InfoPanel.SetActive(isActive);
        }

        public void ChangeHeaderColor(Color newColor)
        {
            Header.color = newColor;
        }

        public void ChangeDetailsColor(Color newColor)
        {
            Details.color = newColor;
        }

        public void ChangeInfoPanelScale(float scale)
        {
            InfoPanel.localScale = new(scale, scale, scale);
        }

        public void ModifyHeaderText(string newHeader)
        {
            Header.text = newHeader;
        }

        public void ModifyDetailsText(string newMessage)
        {
            Details.text = newMessage;
        }

        public void RestoreDefaultInfoPanelText(PieMenuController pieMenu)
        {
            string placeholderHeaderText = "Header";
            string placeholderDetailsText = "Test text";
            ModifyHeaderText(placeholderHeaderText);
            ModifyDetailsText(placeholderDetailsText);
        }

        public void ResetPosition()
        {
            _horizontalPositionOffset = 0;
            _verticalPositionOffset = 0;
            HandlePosition(0, 0);
        }

        public void HandlePosition(int horizontalPosition, int verticalPosition)
        {
            RectTransform rectTransform = _pieMenuController.RectTransform;
            Vector2 anchoredPosition = rectTransform.anchoredPosition;

            anchoredPosition.Set(horizontalPosition, verticalPosition);
            rectTransform.anchoredPosition = anchoredPosition;
            
            float difference = (float) Screen.width / Screen.currentResolution.width;
            anchoredPosition = new(anchoredPosition.x * difference, anchoredPosition.y * difference);
            
            _pieMenuModel.SetAnchoredPosition(anchoredPosition);
        }

        public void ConstraintSelection(bool selectionConstrained)
        {
            _pieMenuModel.SetSelectionConstraintState(selectionConstrained);

            MenuItemSelector selector = _pieMenuController.PieMenuSettingsModel.MenuItemSelector;
            selector.ToggleSelectionConstraint(selectionConstrained);

            if (!selectionConstrained) {
                return;
            }

            float maxDistance = CalculateConstraintMaxDistance(_pieMenuController);
            selector.SetConstraintMaxDistance((int) maxDistance);
        }

        public float CalculateConstraintMaxDistance(PieMenuController pieMenu)
        {
            return pieMenu.PieMenuModel.MenuItemSize * 0.10f;
        }
    }
}